using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.AsNoTracking().ToListAsync());
        }

        public IActionResult Add()
        {
            return View();
        }
        // GET: Users/Add
        public IActionResult AddAdmin()
        {
            return View();
        }
        public async Task<IActionResult> AddFaculty()
        {

            return View(new FacultyVM
            {
                Boards = await getBoards(),
                Standards = await getStandards(),
                Subjects = await getSubjects()
            });
        }
        public async Task<IActionResult> AddStudent()
        {
            return View(new StudentVM
            {
                Boards = await getBoards(),
                Standards = await getStandards(),
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdmin([Bind(include: "FirstName,LastName,Email,Password")] AdminVM user)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var appUser = new ApplicationUser { FirstName = user.FirstName, LastName = user.LastName, AccountType = AccountTypeEnum.Admin, UserName = user.Email, Email = user.Email, EmailConfirmed = true, AdminApproved = true };
                var result = await _userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "Admin");
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFaculty([Bind("FirstName,LastName,Email,Password,BoardId,SubjectId,StandardId")] FacultyVM user)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var subject = await _context.Subjects.FindAsync(user.SubjectId);
                var board = await _context.Boards.FindAsync(user.BoardId);
                var standard = await _context.Standards.FindAsync(user.StandardId);
                var group = await _context.Classes.Where(g => g.BoardId == user.BoardId && g.StandardId == user.StandardId && g.SubjectId == user.SubjectId).FirstAsync();
                if (group == null)
                {
                    ModelState.AddModelError(string.Empty, "Class Does not Exists. Please create one first");
                    await _context.ClassProposals.AddAsync(new ClassProposal { Standard = standard, Subject = subject, Board = board });
                    goto ReturnWithError;
                }
                if (subject != null && board != null && standard != null)
                {
                    var appUser = new ApplicationUser { FirstName = user.FirstName, LastName = user.LastName, AccountType = AccountTypeEnum.Faculty, UserName = user.Email, Email = user.Email, Subject = subject, Board = board, Standard = standard, EmailConfirmed = true, AdminApproved = true };
                    var result = await _userManager.CreateAsync(appUser, user.Password);
                    if (result.Succeeded)
                    {


                        group.Users.Add(appUser);
                        await _userManager.AddToRoleAsync(appUser, "Faculty");
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));

                    }
                    AddErrors(result);
                }
            }
ReturnWithError:
            user.Boards = await getBoards();
            user.Standards = await getStandards();
            user.Subjects = await getSubjects();
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent([Bind("FirstName,LastName,Email,Password,StandardId,BoardId")] StudentVM user)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var standard = await _context.Standards.FindAsync(user.StandardId);
                var board = await _context.Boards.FindAsync(user.BoardId);
                var groups = await _context.Classes.Where(g => g.BoardId == user.BoardId && g.StandardId == user.StandardId).ToListAsync();
                if (groups.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "Class does not Exists. Please create one first");
                    return View(user);
                }
                if (standard != null && board != null)
                {
                    var appUser = new ApplicationUser { FirstName = user.FirstName, LastName = user.LastName, AccountType = AccountTypeEnum.Student, UserName = user.Email, Email = user.Email, Standard = standard, Board = board, EmailConfirmed = true, AdminApproved = true };
                    var result = await _userManager.CreateAsync(appUser, user.Password);
                    if (result.Succeeded)
                    {
                        foreach (var group in groups)
                        {
                            group.Users.Add(appUser);
                        }
                        await _context.SaveChangesAsync();
                        await _userManager.AddToRoleAsync(appUser, "Student");
                        return RedirectToAction(nameof(Index));
                    }
                    AddErrors(result);
                }
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.AccountType == AccountTypeEnum.Admin)
            {
                var adminUser = new AdminVM
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                };
                return View("EditAdmin", adminUser);
            }

            if (user.AccountType == AccountTypeEnum.Faculty)
            {
                var facultyUser = new FacultyVM
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    SubjectId = (int)user.SubjectId,
                    BoardId = (int)user.BoardId,
                    StandardId = (int)user.StandardId,
                    Boards = await getBoards(),
                    Standards = await getStandards(),
                    Subjects = await getSubjects(),
                    isEnabled = user.isEnabled

                };
                return View("EditFaculty", facultyUser);
            }
            else if (user.AccountType == AccountTypeEnum.Student)
            {
                var studentUser = new StudentVM
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    BoardId = (int)user.BoardId,
                    StandardId = (int)user.StandardId,
                    Boards = await getBoards(),
                    Standards = await getStandards(),
                    isEnabled = user.isEnabled
                };
                return View("EditStudent", studentUser);
            }

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(int id, [Bind("Id,FirstName,LastName,Email")] AdminVM user)
        {
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                var appUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
                if (appUser != null)
                {
                    if (user.FirstName != appUser.FirstName)
                    {
                        appUser.FirstName = user.FirstName;
                    }
                    if (user.LastName != appUser.LastName)
                    {
                        appUser.LastName = user.LastName;
                    }
                    if (user.Email != appUser.Email)
                    {
                        if (await _userManager.FindByEmailAsync(user.Email) == null)
                        {
                            var result1 = await _userManager.SetEmailAsync(appUser, user.Email);
                            if (result1.Succeeded)
                            {
                                appUser.Email = user.Email;
                                appUser.EmailConfirmed = true;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Email Already Exists");
                            return View("EditAdmin", user);
                        }
                    }
                    await _userManager.UpdateAsync(appUser);
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "User Not Found");
            }
            return View("EditAdmin", user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFaculty(int id, [Bind("Id,FirstName,LastName,Email,BoardId,SubjectId,StandardId,isEnabled")] FacultyVM user)
        {
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                var appUser = await _context.Users.Include(u => u.Classes).FirstAsync(u => u.Id == user.Id);
                if (appUser != null)
                {
                    var changed = false;
                    if (user.FirstName != appUser.FirstName)
                    {
                        appUser.FirstName = user.FirstName;
                    }
                    if (user.LastName != appUser.LastName)
                    {
                        appUser.LastName = user.LastName;
                    }
                    if (user.isEnabled != appUser.isEnabled)
                    {
                        appUser.isEnabled = user.isEnabled;
                    }
                    if (user.Email != appUser.Email)
                    {
                        if (await _userManager.FindByEmailAsync(user.Email) == null)
                        {
                            var result1 = await _userManager.SetEmailAsync(appUser, user.Email);
                            if (result1.Succeeded)
                            {
                                appUser.Email = user.Email;
                                appUser.EmailConfirmed = true;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Email Already Exists");
                            goto ReturnWithError;
                        }
                    }
                    if (user.LastName != appUser.LastName)
                    {
                        appUser.LastName = user.LastName;
                    }
                    if (user.SubjectId != appUser.SubjectId)
                    {
                        appUser.SubjectId = user.SubjectId;
                        changed = true;
                    }
                    if (user.BoardId != appUser.BoardId)
                    {
                        appUser.BoardId = user.BoardId;
                        changed = true;
                    }
                    if (user.StandardId != appUser.StandardId)
                    {
                        appUser.StandardId = user.StandardId;
                        changed = true;
                    }
                    if (changed)
                    {
                        appUser.Classes.Clear();
                        var classes = await _context.Classes.Where(c => c.BoardId == appUser.BoardId && c.StandardId == appUser.StandardId && c.SubjectId == appUser.SubjectId).ToListAsync();
                        foreach (var classa in classes)
                        {
                            appUser.Classes.Add(classa);
                        }
                    }
                    await _context.SaveChangesAsync();
                    //var result = await _userManager.UpdateAsync(appUser);
                    return RedirectToAction(nameof(Index));
                }
            }
ReturnWithError:
            user.Boards = await getBoards();
            user.Standards = await getStandards();
            user.Subjects = await getSubjects();
            return View("EditFaculty", user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(string id, [Bind("Id,FirstName,LastName,Email,StandardId,BoardId,isEnabled")] StudentVM user)
        {
            if (id == null || id != user.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                var appUser = await _context.Users.Include(u => u.Classes).FirstAsync(u => u.Id == user.Id);
                if (appUser != null)
                {
                    var changed = false;
                    if (user.FirstName != appUser.FirstName)
                    {
                        appUser.FirstName = user.FirstName;
                    }
                    if (user.LastName != appUser.LastName)
                    {
                        appUser.LastName = user.LastName;
                    }
                    if (user.isEnabled != appUser.isEnabled)
                    {
                        appUser.isEnabled = user.isEnabled;
                    }
                    if (user.Email != appUser.Email)
                    {
                        if (await _userManager.FindByEmailAsync(user.Email) == null)
                        {
                            var result1 = await _userManager.SetEmailAsync(appUser, user.Email);
                            if (result1.Succeeded)
                            {
                                appUser.Email = user.Email;
                                appUser.EmailConfirmed = true;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Email Already Exists");
                            goto ReturnWithError;
                        }
                    }
                    if (user.StandardId != appUser.StandardId)
                    {
                        appUser.StandardId = user.StandardId;
                        changed = true;
                    }
                    if (user.BoardId != appUser.BoardId)
                    {
                        appUser.BoardId = user.BoardId;
                        changed = true;
                    }
                    if (changed)
                    {
                        appUser.Classes.Clear();
                        var classes = await _context.Classes.Where(c => c.BoardId == appUser.BoardId && c.StandardId == appUser.StandardId).ToListAsync();
                        foreach (var classa in classes)
                        {
                            appUser.Classes.Add(classa);
                        }
                    }
                    await _context.SaveChangesAsync();
                    //if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                    //else ModelState.AddModelError(string.Empty, result.Errors.First().Description);
                }
            }
ReturnWithError:
            user.Boards = await getBoards();
            user.Standards = await getStandards();

            return View("EditStudent", user);
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
                return NotFound();
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return BadRequest();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                return Json($"Email {email} is already in use.");
            }

            return Json(true);
        }



        public async Task<SelectList> getBoards(int? boardId = null)
        {
            return new SelectList(await _context.Boards.OrderBy(b => b.AbbrName).AsNoTracking().ToListAsync(), "Id", "Name", boardId);
        }
        public async Task<SelectList> getSubjects(int? subjectId = null)
        {
            return new SelectList(await _context.Subjects.OrderBy(b => b.Name).AsNoTracking().ToListAsync(), "Id", "Name", subjectId);
        }
        public async Task<SelectList> getStandards(int? standardId = null)
        {
            return new SelectList(await _context.Standards.OrderBy(b => b.Level).AsNoTracking().ToListAsync(), "Id", "Name", standardId);
        }

        private bool userExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
