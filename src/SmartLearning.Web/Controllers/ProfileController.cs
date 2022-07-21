using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Controllers
{
    [Authorize]
    [Route("/Profile")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Route("{id?}", Name = "ViewProfile")]
        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            if (id == null)
            {
                return View("Error");
            }
            var user = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
                return View("Error");
            var vm = new ProfileViewModel
            {
                User = user,
            };
            return View(vm);
        }

        [Route("/MyProfile", Name = "MyProfile")]
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return View("Error");
            var vm = new ProfileViewModel
            {
                User = user,
            };
            return View("Index", vm);
        }
    }
}
