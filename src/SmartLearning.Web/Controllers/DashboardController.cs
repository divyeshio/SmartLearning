using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            switch (Enum.Parse(typeof(AccountTypeEnum), User.FindFirst(ClaimTypes.Role).Value))
            {
                case AccountTypeEnum.Admin:
                    {
                        var vm = new AdminDashboardViewModel
                        {
                            User = user,
                            AdminCount = await _context.Users.Where(u => u.AccountType == AccountTypeEnum.Admin).CountAsync(),
                            FacultyCount = await _context.Users.Where(u => u.AccountType == AccountTypeEnum.Faculty).CountAsync(),
                            StudentsCount = await _context.Users.Where(u => u.AccountType == AccountTypeEnum.Student).CountAsync(),
                            ClassesCount = await _context.Classes.CountAsync(),
                            TestCount = await _context.Tests.CountAsync(),
                            NotesCount = await _context.Notes.CountAsync(),
                            SamplePapersCount = await _context.SamplePapers.CountAsync(),
                            ReferenceBookCount = await _context.ReferenceBooks.CountAsync(),
                            BoardsCount = await _context.Boards.CountAsync(),
                            StandardsCount = await _context.Standards.CountAsync(),
                            SubjectsCount = await _context.Subjects.CountAsync(),
                        };
                        return View("AdminDashboard", vm);
                    }
                case AccountTypeEnum.Faculty:
                    {
                        var vm = new AdminDashboardViewModel
                        {
                            User = user,
                            AdminCount = await _context.Users.Where(u => u.AccountType == AccountTypeEnum.Admin).CountAsync(),
                            FacultyCount = await _context.Users.Where(u => u.AccountType == AccountTypeEnum.Faculty).CountAsync(),
                            StudentsCount = await _context.Users.Where(u => u.AccountType == AccountTypeEnum.Student).CountAsync(),
                            ClassesCount = await _context.Classes.CountAsync(),
                            TestCount = await _context.Tests.CountAsync(),
                            NotesCount = await _context.Notes.CountAsync(),
                            SamplePapersCount = await _context.SamplePapers.CountAsync(),
                            ReferenceBookCount = await _context.ReferenceBooks.CountAsync(),
                            BoardsCount = await _context.Boards.CountAsync(),
                            StandardsCount = await _context.Standards.CountAsync(),
                            SubjectsCount = await _context.Subjects.CountAsync(),
                        };
                        return View("FacultyDashboard", vm);
                    }
                case AccountTypeEnum.Student:
                    return RedirectToAction("Index", "Chat");
            }

            return NotFound();
        }

        [HttpGet]
        [Route("/MyStudents", Name = "MyStudents")]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> MyStudents()
        {
            return View(await _context.Users.Where(u => u.AccountType == AccountTypeEnum.Student && u.BoardId == int.Parse(User.FindFirst("BoardId").Value) && u.StandardId == int.Parse(User.FindFirst("StandardId").Value)).ToListAsync());
        }

        [HttpGet]
        [Route("/MyCoWorkers", Name = "MyCoWorkers")]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> MyCoWorkers()
        {
            var user = await _userManager.GetUserAsync(User);
            var users = await _context.Users.Where(u => u.AccountType == AccountTypeEnum.Faculty && u.BoardId == int.Parse(User.FindFirst("BoardId").Value) && u.StandardId == int.Parse(User.FindFirst("StandardId").Value) && u.AdminApproved == true).Include(u => u.Subject).ToListAsync();
            users.Remove(user);
            return View(users);
        }

        [HttpGet]
        [Route("/MyFaculties", Name = "MyFaculties")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyFaculties()
        {
            return View(await _context.Users.Where(u => u.AccountType == AccountTypeEnum.Faculty && u.BoardId == int.Parse(User.FindFirst("BoardId").Value) && u.StandardId == int.Parse(User.FindFirst("StandardId").Value) && u.AdminApproved == true).Include(u => u.Classes).ToListAsync());
        }

    }
}
