using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;

namespace SmartLearning.Web.Controllers
{
  [Authorize(Roles = "Admin")]
  public class ApprovalController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public ApprovalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    // GET: ApprovalController
    public async Task<ActionResult> Index()
    {

      return View(await _context.Users.Include(u => u.Board).Include(u => u.Subject).Include(u => u.Standard).Where(b => b.AdminApproved == null && b.AccountType == AccountTypeEnum.Faculty && b.EmailConfirmed == true).ToListAsync());
    }

    // GET: ApprovalController/Details/5

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
      var user = await _context.Users.FindAsync(id);
      if (user == null)
      {
        return Redirect("Index");
      }
      user.AdminApproved = true;
      await _userManager.AddToRoleAsync(user, "Faculty");
      _context.Update(user);
      var group = await _context.Classes.Where(g => g.BoardId == user.BoardId && g.StandardId == user.StandardId && g.SubjectId == user.SubjectId).FirstAsync();
      group.Users.Add(user);
      await _context.SaveChangesAsync();
      return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deny(int id)
    {
      var user = await _context.Users.FindAsync(id);
      if (user == null)
      {
        return Redirect("Index");
      }
      user.AdminApproved = false;
      _context.Update(user);
      await _context.SaveChangesAsync();
      return RedirectToAction("Index");
    }


  }
}
