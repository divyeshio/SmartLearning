using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Core.Interfaces;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO.ManageViewModels;

namespace SmartLearning.Web.Controllers
{
  [Authorize]
  public class ManageAccountController : Controller
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;
    private IWebHostEnvironment _env;

    public ManageAccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IEmailSender emailSender,
    ApplicationDbContext context,
    IWebHostEnvironment webHostEnvironment,
    ILoggerFactory loggerFactory)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _emailSender = emailSender;
      _context = context;
      _env = webHostEnvironment;
      _logger = loggerFactory.CreateLogger<ManageAccountController>();
    }

    //
    // GET: /Manage/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound();
      }
      var accType = user.AccountType;
      var board = "";
      var standard = "";
      var subject = "";
      if (user.AccountType == AccountTypeEnum.Student)
      {
        board = await _context.Boards.Where(b => b.Id == user.BoardId).Select(b => b.AbbrName).AsNoTracking().FirstOrDefaultAsync();
        standard = await _context.Standards.Where(b => b.Id == user.StandardId).Select(b => b.DisplayName).AsNoTracking().FirstOrDefaultAsync();
      }
      if (user.AccountType == AccountTypeEnum.Faculty)
      {
        board = await _context.Boards.Where(b => b.Id == user.BoardId).Select(b => b.AbbrName).AsNoTracking().FirstOrDefaultAsync();
        standard = await _context.Standards.Where(b => b.Id == user.StandardId).Select(b => b.DisplayName).AsNoTracking().FirstOrDefaultAsync();
        subject = await _context.Subjects.Where(b => b.Id == user.SubjectId).Select(b => b.Name).AsNoTracking().FirstOrDefaultAsync();
      }
      var model = new IndexViewModel
      {
        PhoneNumber = user.PhoneNumber,
        Avatar = user.Avatar,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        AccountType = accType.ToString(),
        Board = board,
        Standard = standard,
        Subject = subject
      };
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index([Bind("FirstName,LastName,Email,AvatarFile,PhoneNumber")] IndexViewModel vm)
    {
      var accType = "";
      if (ModelState.IsValid)
      {
        var user = await _userManager.GetUserAsync(User);
        if (vm.AvatarFile != null)
        {
          user.Avatar = await UploadedFile(vm.AvatarFile);
          await _userManager.RemoveClaimAsync(user, User.FindFirst("avatar"));
          var claim = new Claim("avatar", user.Avatar);
          await _userManager.AddClaimAsync(user, claim);
        }
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (vm.PhoneNumber != phoneNumber)
        {
          var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, vm.PhoneNumber);
          if (!setPhoneResult.Succeeded)
          {
            ModelState.AddModelError(string.Empty, "Unexpected error when trying to set phone number.");
            accType = user.AccountType.ToString();
            goto ReturnWithError;
          }
        }
        if (user.FirstName != vm.FirstName)
        {
          user.FirstName = vm.FirstName;
        }
        if (user.LastName != vm.LastName)
        {
          user.LastName = vm.LastName;
        }
        //await _context.SaveChangesAsync();
        await _userManager.UpdateAsync(user);
        await _signInManager.RefreshSignInAsync(user);
        return RedirectToAction(nameof(Index));
      }
ReturnWithError:
      vm.AccountType = accType;
      return View(vm);
    }

    private Task<string> UploadedFile(IFormFile file)
    {
      string uniqueFileName = null;

      if (file != null)
      {
        var uploadsFolder = Path.Combine(_env.ContentRootPath, "StaticFiles", "avatars");
        uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          file.CopyTo(fileStream);
        }
      }
      return Task.FromResult(uniqueFileName);
    }



    [HttpPost]
    public async Task<IActionResult> RemoveAvatar()
    {
      var user = await _userManager.GetUserAsync(User);

      if (await RemoveFile(user.Avatar))
      {
        await _userManager.RemoveClaimAsync(user, User.FindFirst("avatar"));
        user.Avatar = null;
        var claim = new Claim("avatar", "default.jpg");
        await _userManager.AddClaimAsync(user, claim);
        await _userManager.UpdateAsync(user);
        await _signInManager.RefreshSignInAsync(user);
      }
      return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Password()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
      }

      var hasPassword = await _userManager.HasPasswordAsync(user);
      if (!hasPassword)
      {
        return View("SetPassword");
      }
      return View("ChangePassword");
    }
    // POST: /Manage/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var user = await GetCurrentUserAsync();
      if (user != null)
      {
        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (result.Succeeded)
        {
          await _signInManager.SignInAsync(user, isPersistent: false);
          _logger.LogInformation(3, "User changed their password successfully.");
          return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
        }
        AddErrors(result);
        return View(model);
      }
      return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var user = await GetCurrentUserAsync();
      if (user != null)
      {
        var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
        if (result.Succeeded)
        {
          await _signInManager.SignInAsync(user, isPersistent: false);
          return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
        }
        AddErrors(result);
        return View(model);
      }
      return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
    }


    [HttpGet]
    public async Task<IActionResult> ExternalLogins()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound($"Unable to load user with ID 'user.Id'.");
      }
      var Info = new List<UserInfo>();
      foreach (var item in await _userManager.GetLoginsAsync(user))
      {
        var claims = await _userManager.GetClaimsAsync(user);
        var c = claims.Where(a => a.Type == "urn:google:picture").FirstOrDefault();
        if (c != null)
          Info.Add(new UserInfo { Info = item, ProfileUrl = c.Value });
        else
          Info.Add(new UserInfo { Info = item, ProfileUrl = $"/StaticFiles/avatars/{User.FindFirstValue("avatar")}" });
      }
      var vm = new ExternalLoginsViewModel
      {
        CurrentLogins = Info
      };
      vm.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
          .Where(auth => vm.CurrentLogins.All(ul => auth.Name != ul.Info.LoginProvider))
          .ToList();
      vm.ShowRemoveButton = user.PasswordHash != null || vm.CurrentLogins.Count > 1;
      return View(vm);
    }

    // POST: /Manage/LinkLogin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LinkLogin(string provider)
    {
      // Request a redirect to the external login provider to link a login for the current user
      var redirectUrl = Url.Action("LinkLoginCallback", "ManageAccount");
      var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
      return Challenge(properties, provider);
    }

    //
    // GET: /Manage/LinkLoginCallback
    [HttpGet]
    public async Task<ActionResult> LinkLoginCallback()
    {
      var user = await GetCurrentUserAsync();
      if (user == null)
      {
        return View("Error");
      }
      var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
      if (info == null)
      {
        return RedirectToAction(nameof(ExternalLogins), new { Message = ManageMessageId.Error });
      }
      if (info.Principal.HasClaim(c => c.Type == "urn:google:picture"))
      {
        await _userManager.AddClaimAsync(user,
            info.Principal.FindFirst("urn:google:picture"));
      }
      var result = await _userManager.AddLoginAsync(user, info);
      var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
      return RedirectToAction(nameof(ExternalLogins), new { Message = message });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
    {
      ManageMessageId? message = ManageMessageId.Error;
      var user = await GetCurrentUserAsync();
      if (user != null)
      {
        var result = await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
        if (result.Succeeded)
        {
          await _signInManager.SignInAsync(user, isPersistent: false);
          message = ManageMessageId.RemoveLoginSuccess;
        }
      }
      return RedirectToAction(nameof(ExternalLogins), new { Message = message });
    }


    [HttpGet]
    public async Task<IActionResult> TwoFactorAuthentication()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
      }
      var vm = new TwoFactorAuthenticationViewModel
      {
        HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
        Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
        IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
        RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
      };
      return View(vm);
    }

    #region Helpers

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }
    }

    public enum ManageMessageId
    {
      AddPhoneSuccess,
      AddLoginSuccess,
      ChangePasswordSuccess,
      SetTwoFactorSuccess,
      SetPasswordSuccess,
      RemoveLoginSuccess,
      RemovePhoneSuccess,
      Error
    }

    private Task<bool> RemoveFile(string filename)
    {
      var fullPath = Path.Combine(_env.ContentRootPath, "StaticFiles", "avatars", filename);
      if (System.IO.File.Exists(fullPath))
      {
        System.IO.File.Delete(fullPath);
        return Task.FromResult(true);
      }
      return Task.FromResult(false);
    }

    private async Task<ApplicationUser> GetCurrentUserAsync()
    {
      return await _userManager.GetUserAsync(HttpContext.User);
    }

    #endregion
  }
}
