using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.Infrastructure.Data;

namespace SmartLearning.Web.Controllers
{
  public class StandardsController : Controller
  {
    private readonly ApplicationDbContext _context;

    public StandardsController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: Standards
    public async Task<IActionResult> Index()
    {
      return View(await _context.Standards.OrderBy(s => s.Level).ToListAsync());
    }


    // GET: Standards/Create
    public IActionResult Add()
    {
      return View();
    }

    // POST: Standards/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind("Name")] Standard standard)
    {
      ModelState.Remove("DisplayName");
      if (ModelState.IsValid)
      {
        if (!await _context.Standards.AnyAsync(s => s.Level == standard.Level))
        {
          standard.DisplayName = standard.Level.ToString();
          await _context.Standards.AddAsync(standard);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
        else
        {
          ModelState.AddModelError(string.Empty, "Standard Already Exists");
          return View(standard);
        }
      }
      return View(standard);
    }

    // GET: Standards/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var standard = await _context.Standards.FirstOrDefaultAsync(s => s.Id == id);
      if (standard == null)
      {
        return NotFound();
      }
      return View(standard);
    }

    // POST: Standards/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Standard standard)
    {
      if (id != standard.Id)
      {
        return NotFound();
      }
      ModelState.Remove("DisplayName");
      if (ModelState.IsValid)
      {
        try
        {
          if (!await _context.Standards.AnyAsync(s => s.Level == standard.Level))
          {
            standard.DisplayName = standard.Level.ToString();
            _context.Update(standard);
            await _context.SaveChangesAsync();
          }
          else
          {
            ModelState.AddModelError(string.Empty, "Standard Already Exists");
            return View(standard);
          }
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!await StandardExists(standard.Id))
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
      return View(standard);
    }


    // POST: Standards/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
      var standard = await _context.Standards.FindAsync(id);
      _context.Standards.Remove(standard);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private async Task<bool> StandardExists(int id)
    {
      return await _context.Standards.AnyAsync(e => e.Id == id);
    }
  }
}
