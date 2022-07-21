using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Core.Interfaces;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO.AccountViewModels;

namespace SmartLearning.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        //private readonly ISmsSender _smsSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            //ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            //_smsSender = smsSender;
            _roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _context = context;
        }

        /*[HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Initialize()
        {

            await _context.Subjects.AddAsync(new Subject { Name = "English" });
            await _context.Subjects.AddAsync(new Subject { Name = "Maths" });
            await _context.Subjects.AddAsync(new Subject { Name = "Science" });
            await _context.Subjects.AddAsync(new Subject { Name = "Economics" });
            foreach (var value in Enumerable.Range(1, 12))
            {
                _context.Standards.Add(new Standard { Name = value, DisplayName = $"{value}" });
            };
            await _context.Boards.AddAsync(new Board { AbbrName = "ICSE" });
            await _context.Boards.AddAsync(new Board { AbbrName = "CBSE" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "Student" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "Faculty" });
            await _context.SaveChangesAsync();
            var subject = await _context.Subjects.FirstAsync();
            var standard = await _context.Standards.Where(s => s.Name == 11).FirstAsync();
            var board = await _context.Boards.Where(b => b.AbbrName == "ICSE").FirstAsync();
            var AdminUser = new ApplicationUser { FirstName = "Admin", LastName = "Smart Learning", UserName = "admin@smartlearning.com", Email = "admin@smartlearning.com", AdminApproved = true, AccountType = AccountTypeEnum.Admin, EmailConfirmed = true };
            var StudentUser = new ApplicationUser { FirstName = "Student", LastName = "Smart", UserName = "student@smartlearning.com", Email = "student@smartlearning.com", AccountType = AccountTypeEnum.Student, EmailConfirmed = true, Board = board, Standard = standard };
            await _userManager.CreateAsync(AdminUser, "Admin@123");
            await _userManager.AddToRoleAsync(AdminUser, "Admin");
            await _userManager.CreateAsync(StudentUser, "Student@123");
            await _userManager.AddToRoleAsync(StudentUser, "Student");
            var groups = new List<Class>();
            var standards = await _context.Standards.ToListAsync();
            var subjects = await _context.Subjects.ToListAsync();
            var boards = await _context.Boards.ToListAsync();
            foreach (var subject1 in subjects)
            {
                foreach (var standard1 in standards)
                {
                    foreach (var board1 in boards)
                    {
                        var group = new Class { Name = Class.GenerateGroupName(board1.AbbrName, standard1.DisplayName, subject1.Name), Board = board1, Standard = standard1, Subject = subject1 };
                        groups.Add(group);

                    }

                }
            }
            await _context.Classes.AddRangeAsync(groups);
            await _context.SaveChangesAsync();
            var groups1 = await _context.Classes.Where(g => g.BoardId == StudentUser.BoardId && g.StandardId == StudentUser.StandardId).ToListAsync();
            foreach (var group in groups1)
            {
                group.Users.Add(StudentUser);
            }
            await _context.SaveChangesAsync();
            *//*foreach(int value in Enumerable.Range(1, 100))
            {
                await _userManager.CreateAsync(new ApplicationUser { FirstName = "Admin", LastName = "Smart Learning", UserName = $"admin{value.ToString()}@smartlearning.com", Email = $"admin{value.ToString()}@smartlearning.com", AdminApproved = true, AccountType = AccountTypeEnum.Admin, EmailConfirmed = true }, "Admin@123");
            };*//*
            //_context.Users.RemoveRange(_context.Users.ToList());

            return RedirectToAction("Login");
        }*/

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ExternalLogins = await getExternalLogins() });
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!user.isEnabled)
                    {
                        ModelState.AddModelError(string.Empty, "Login disabled.");
                        goto ReturnWithError;
                    }
                    if (!user.EmailConfirmed)
                    {
                        ModelState.AddModelError(string.Empty, "Email Not Confirmed yet");
                        goto ReturnWithError;
                    }
                    if (user.AccountType == AccountTypeEnum.Faculty)
                    {
                        if (user.AdminApproved == null)
                        {
                            ModelState.AddModelError(string.Empty, "Wait For Approval");
                            goto ReturnWithError;
                        }
                    }
                    var result = await _signInManager.PasswordSignInAsync(user: user, password: model.Password, isPersistent: model.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        if (user.AccountType == AccountTypeEnum.Student && user.FaceDataId == null)
                            return RedirectToAction("AddFaceData", "Live");
                        _logger.LogInformation(1, model.Email + " logged in.");
                        return ReturnUrlRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, model.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning(2, "User account locked out.");
                        return View("Lockout");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
ReturnWithError:
            model.ExternalLogins = await getExternalLogins();
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel { Boards = await getBoards(), Standards = await getStandards(), ExternalLogins = await getExternalLogins() });
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterFaculty(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterFacultyViewModel { Boards = await getBoards(), Standards = await getStandards(), Subjects = await getSubjects(), ExternalLogins = await getExternalLogins() });
        }


        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var standard = await _context.Standards.FindAsync(model.StandardId);
                var board = await _context.Boards.FindAsync(model.BoardId);
                if (board == null || standard == null)
                {
                    return BadRequest();
                }
                var classExists = await _context.Classes.Where(g => g.BoardId == model.BoardId && g.StandardId == model.StandardId).FirstOrDefaultAsync();
                if (classExists == null)
                {
                    ModelState.AddModelError(string.Empty, "Registrations are closed.Please Try Again Later");
                    var proposal = await _context.ClassProposals.Where(cp => cp.Standard == standard && cp.Board == board).FirstOrDefaultAsync();
                    if (proposal == null)
                        await _context.ClassProposals.AddAsync(new ClassProposal { Standard = standard, Board = board });
                    else proposal.Count += 1;
                    await _context.SaveChangesAsync();
                    goto ReturnWithError;
                }
                if (!classExists.IsRegistrationAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Registrations are closed.Please Try Again Later");
                    goto ReturnWithError;
                }
                var user = new ApplicationUser { FirstName = model.FirstName, LastName = model.LastName, UserName = model.Email, Email = model.Email, AdminApproved = true, Standard = standard, Board = board, AccountType = AccountTypeEnum.Student };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Send an email with this link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                        $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a>");
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation", new { email = model.Email });
                    }
                    else
                    {
                        return LocalRedirect(returnUrl == null ? "/Account/Login" : returnUrl);
                    }
                }
                AddErrors(result);
            }
ReturnWithError:
            model.Boards = await getBoards();
            model.Standards = await getStandards();
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterFaculty(RegisterFacultyViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var subject = await _context.Subjects.FindAsync(model.SubjectId);
                var board = await _context.Boards.FindAsync(model.BoardId);
                var standard = await _context.Standards.FindAsync(model.StandardId);
                if (subject == null || board == null || standard == null)
                {
                    return BadRequest();
                }
                var groupExists = await _context.Classes.Where(g => g.BoardId == model.BoardId && g.SubjectId == model.SubjectId && g.StandardId == model.StandardId).FirstOrDefaultAsync();
                if (groupExists == null)
                {
                    ModelState.AddModelError(string.Empty, "Registrations are closed.Please Try Again Later");
                    var proposal = await _context.ClassProposals.Where(cp => cp.Standard == standard && cp.Board == board).FirstOrDefaultAsync();
                    if (proposal == null)
                        await _context.ClassProposals.AddAsync(new ClassProposal { Standard = standard, Board = board, Subject = subject });
                    else proposal.Count += 1;
                    await _context.SaveChangesAsync();
                    goto ReturnWithError;
                }
                if (!groupExists.IsRegistrationAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Registrations are closed.Please Try Again Later");
                    goto ReturnWithError;
                }
                var user = new ApplicationUser { FirstName = model.FirstName, LastName = model.LastName, UserName = model.Email, Email = model.Email, SubjectId = model.SubjectId, BoardId = model.BoardId, StandardId = model.StandardId, AccountType = AccountTypeEnum.Faculty };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Send an email with this link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                        $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a>");

                    await _context.SaveChangesAsync();
                    _logger.LogInformation(3, "User created a new account with password.");
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation", new { email = model.Email });
                    }
                    else
                    {
                        return LocalRedirect(returnUrl == null ? "/Index" : returnUrl);
                    }
                }
                ModelState.AddModelError(string.Empty, result.Errors.First().Description);
            }
ReturnWithError:
            model.Boards = await getBoards();
            model.Standards = await getStandards();
            model.Subjects = await getSubjects();
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterConfirmation(string email)
        {
            if (email == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }
            var model = new RegisterConfirmationViewModel();
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            model.DisplayConfirmAccountLink = true;
            if (model.DisplayConfirmAccountLink)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                model.EmailConfirmationUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
            }

            return View(model);
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (email != null && user != null)
            {
                if (!user.isEnabled)
                    return RedirectToAction(nameof(Login));
                if (user.AccountType == AccountTypeEnum.Faculty)
                {
                    var isApproved = user.AdminApproved ?? false;
                    if (!isApproved)
                        return RedirectToAction(nameof(Login));
                }
            }
            if (info.Principal.HasClaim(c => c.Type == "urn:google:picture"))
            {
                await _userManager.AddClaimAsync(user,
                    info.Principal.FindFirst("urn:google:picture"));
            }
            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                // Update any authentication tokens if login succeeded
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            if (email != null)
            {
                if (user == null)
                {
                    // If the user does not have an account, then ask the user to create an account.
                    ViewData["ReturnUrl"] = returnUrl;
                    ViewData["ProviderDisplayName"] = info.ProviderDisplayName;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName), LastName = info.Principal.FindFirstValue(ClaimTypes.Surname), Email = email, Boards = await getBoards(), Standards = await getStandards(), Subjects = await getSubjects() });
                }
                else
                {
                    var check = await _userManager.IsEmailConfirmedAsync(user);
                    if (!check)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                    }

                    //Local Account Exists therefore add this login
                    var res = await _userManager.AddLoginAsync(user, info);
                    //if(res.Succeeded)
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }
            else
            {
                // Modified Cookie So return Error
                return NotFound();
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != model.Email)
                {
                    return NotFound();
                }
                var board = await _context.Boards.AnyAsync(s => s.Id == model.BoardId);
                var standard = await _context.Standards.AnyAsync(s => s.Id == model.StandardId);
                if (board && standard)
                {
                    var user = new ApplicationUser { UserName = email, Email = email, FirstName = model.FirstName, LastName = model.LastName, StandardId = model.StandardId, BoardId = model.BoardId, EmailConfirmed = true };
                    if (model.SubjectId == null)
                    {
                        user.AccountType = AccountTypeEnum.Student;
                    }
                    else
                    {
                        var subject = await _context.Subjects.AnyAsync(s => s.Id == model.SubjectId);
                        var groupExists = await _context.Classes.Where(g => g.BoardId == model.BoardId && g.SubjectId == model.SubjectId && g.StandardId == model.StandardId).FirstOrDefaultAsync();
                        if (groupExists == null || !subject)
                        {
                            ModelState.AddModelError(string.Empty, "Registration for this class is currently closed");
                            return View(model);
                        }
                        user.AccountType = AccountTypeEnum.Faculty;
                        user.SubjectId = (int?)model.SubjectId;
                    }
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {
                            if (info.Principal.HasClaim(c => c.Type == "urn:google:picture"))
                            {
                                await _userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst("urn:google:picture"));
                            }
                            if (user.AccountType == AccountTypeEnum.Student)
                            {
                                await _userManager.AddToRoleAsync(user, user.AccountType.ToString());
                                await AddToClasses(user);
                                await _signInManager.SignInAsync(user, isPersistent: false);
                            }
                            _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                            // Update any authentication tokens as well
                            await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                            return RedirectToLocal(returnUrl);
                        }
                        AddErrors(result);
                    }
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        private async Task AddToClasses(ApplicationUser user)
        {
            var classes = await _context.Classes.Where(g => g.BoardId == user.BoardId && g.StandardId == user.StandardId).ToListAsync();
            foreach (var classa in classes)
            {
                classa.Users.Add(user);
            }
            await _context.SaveChangesAsync();
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, user.AccountType.ToString());
                if (user.AccountType == AccountTypeEnum.Student)
                {
                    await AddToClasses(user);
                }

                return View("ConfirmEmail");
            }
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendEmail()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmail(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ResendEmailConfirmation));
                }
                // Send an email with this link
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                return RedirectToAction(nameof(ResendEmailConfirmation));
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            if (model.SelectedProvider == "Authenticator")
            {
                return RedirectToAction(nameof(VerifyAuthenticatorCode), new { model.ReturnUrl, model.RememberMe });
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                //await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        //
        // GET: /Account/VerifyAuthenticatorCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyAuthenticatorCode(bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyAuthenticatorCodeViewModel { ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyAuthenticatorCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyAuthenticatorCode(VerifyAuthenticatorCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        //
        // GET: /Account/UseRecoveryCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> UseRecoveryCode(string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new UseRecoveryCodeViewModel { ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/UseRecoveryCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UseRecoveryCode(UseRecoveryCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(model.Code);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult NotAuthorized()
        {
            return View();
        }

        public async Task<SelectList> getBoards(int? boardId = null)
        {
            return new SelectList(await _context.Boards.OrderBy(b => b.AbbrName).AsNoTracking().ToListAsync(), "Id", "Name", boardId);
        }
        public async Task<SelectList> getSubjects(int? subjectId = null)
        {
            return new SelectList(await _context.Subjects.OrderBy(b => b.Name).AsNoTracking().ToListAsync(), "Id", "Name", subjectId);
        }
        public async Task<SelectList> getStandards(long? standardId = null)
        {
            return new SelectList(await _context.Standards.OrderBy(b => b.Level).AsNoTracking().ToListAsync(), "Id", "Name", standardId);
        }
        public async Task<List<AuthenticationScheme>> getExternalLogins()
        {
            return (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }



        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult ReturnUrlRedirect(string returnUrl)
        {
            if (returnUrl == null)
                return RedirectToAction("Index", "Dashboard");
            else
                return RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(DashboardController.Index), "Dashboard");
            }
        }

        #endregion
    }
}
