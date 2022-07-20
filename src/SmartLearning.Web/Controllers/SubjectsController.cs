using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.Infrastructure.Data;

namespace SmartLearning.Web.Controllers
{
  public class SubjectsController : Controller
  {
    private readonly ApplicationDbContext _context;

    public SubjectsController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: Subjects
    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> Index()
    {
      return View(await _context.Subjects.AsNoTracking().ToListAsync());
    }

    // GET: Subjects/Details/5
    public async Task<IActionResult> Details(long? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var subject = await _context.Subjects
          .AsNoTracking()
          .FirstOrDefaultAsync(m => m.Id == id);
      if (subject == null)
      {
        return NotFound();
      }

      return View(subject);
    }

    // GET: Subjects/Create
    public IActionResult Add()
    {
      return View();
    }

    // POST: Subjects/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind("Id,Name,Details")] Subject subject)
    {
      if (ModelState.IsValid)
      {
        _context.Add(subject);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(subject);
    }

    // GET: Subjects/Edit/5
    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> Edit(long? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var subject = await _context.Subjects.FindAsync(id);
      if (subject == null)
      {
        return NotFound();
      }
      return View(subject);
    }

    // POST: Subjects/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Details")] Subject subject)
    {
      if (id != subject.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(subject);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!SubjectExists(subject.Id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      return View(subject);
    }


    // POST: Subjects/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
      var subject = await _context.Subjects.FindAsync(id);
      _context.Subjects.Remove(subject);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool SubjectExists(long id)
    {
      return _context.Subjects.Any(e => e.Id == id);
    }
  }
}
