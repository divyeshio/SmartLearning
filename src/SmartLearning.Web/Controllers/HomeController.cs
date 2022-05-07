using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartLearning.Controllers
{
  [AllowAnonymous]
  public class HomeController : Controller
  {

    [HttpGet]
    public IActionResult Index()
    {
      return View();
    }

  }
}