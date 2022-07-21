using Microsoft.AspNetCore.Mvc;

namespace SmartLearning.Web.Controllers
{
  public class PageController : Controller
  {
    [Route("/FAQ", Name = "FAQ")]
    [HttpGet]
    public IActionResult FAQ()
    {
      return View();
    }

    [Route("/AboutUs", Name = "AboutUs")]
    [HttpGet]
    public IActionResult AboutUs()
    {
      return View();
    }

    [Route("/PrivacyPolicy", Name = "PrivacyPolicy")]
    [HttpGet]
    public IActionResult PrivacyPolicy()
    {
      return View();
    }

    [Route("/TermsAndConditions", Name = "TermsAndConditions")]
    [HttpGet]
    public IActionResult TermsAndConditions()
    {
      return View();
    }
  }
}
