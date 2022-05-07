using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Data;
using SmartLearning.ViewModels;

namespace SmartLearning.Controllers
{
  [Authorize]
  [Route("/Profile")]
  public class ProfileController : Controller
  {
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
      _context = context;
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
      return View();
    }

    [Route("/MyProfile", Name = "MyProfile")]
    [HttpGet]
    public IActionResult MyProfile()
    {


      return View("Index");
    }
  }
}
