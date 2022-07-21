using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Infrastructure.Data;

namespace SmartLearning.Web.Controllers
{
  [Authorize(Roles = "Admin")]
  public class ClassController : Controller
  {
    private readonly ApplicationDbContext _context;

    public ClassController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: Classes
    public async Task<IActionResult> Index(long? subject, long? board, int? standard)
    {
      ViewData["Boards"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name", board);
      ViewData["Standards"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Name", standard);
      ViewData["Subjects"] = new SelectList(_context.Subjects.OrderBy(b => b.Name), "Id", "Name", subject);
      var classes = from s in _context.Classes
                    select s;
      if (subject != null)
      {
        classes = classes.Where(s => s.SubjectId == subject);
      }
      if (board != null)
      {
        classes = classes.Where(c => c.BoardId == board);
      }
      if (standard != null)
      {
        classes = classes.Where(c => c.StandardId == standard);
      }
      return View(await classes.OrderByDescending(c => c.Name).Include(g => g.Board).Include(g => g.Standard).Include(g => g.Subject).ToListAsync());
    }

    // GET: Classes/Details/5
    public async Task<IActionResult> Details(int id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var group = await _context.Classes
          .Include(g => g.Board)
          .Include(g => g.Standard)
          .Include(g => g.Subject)
          .Include(g => g.Users)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (group == null)
      {
        return NotFound();
      }

      return View(group);
    }

    // GET: Classes/Add
    public IActionResult Add()
    {
      ViewData["BoardId"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name");
      ViewData["StandardId"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Name");
      ViewData["SubjectId"] = new SelectList(_context.Subjects.OrderBy(b => b.Name), "Id", "Name");
      return View();
    }

    // POST: Classes/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind("BoardId,StandardId,SubjectId")] Classroom group)
    {
      var board = await _context.Boards.FindAsync(group.BoardId);
      var standard = await _context.Standards.FindAsync(group.StandardId);
      var subject = await _context.Subjects.FindAsync(group.SubjectId);

      if (board != null && standard != null && subject != null)
      {
        ModelState.Remove("Name");
        if (await _context.Classes.Where(c => c.StandardId == standard.Id && c.SubjectId == subject.Id && c.BoardId == board.Id).FirstOrDefaultAsync() != null)
        {
          ViewData["BoardId"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name", group.BoardId);
          ViewData["StandardId"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Name", group.StandardId);
          ViewData["SubjectId"] = new SelectList(_context.Subjects.OrderBy(b => b.Name), "Id", "Name", group.SubjectId);
          ModelState.AddModelError(string.Empty, "Class Already Exists");
          return View(group);
        }

        group.Name = Classroom.GenerateGroupName(board.AbbrName, standard.DisplayName, subject.Name);
        if (ModelState.IsValid)
        {
          _context.Add(group);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
      }
      else
      {
        ModelState.AddModelError(string.Empty, "Invalid Details");
      }
      ViewData["BoardId"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name", group.BoardId);
      ViewData["StandardId"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Name", group.StandardId);
      ViewData["SubjectId"] = new SelectList(_context.Subjects.OrderBy(b => b.Name), "Id", "Name", group.SubjectId);
      return View(group);
    }

    // GET: Classes/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var group = await _context.Classes.FindAsync(id);
      if (group == null)
      {
        return NotFound();
      }
      ViewData["BoardId"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name", group.BoardId);
      ViewData["StandardId"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Id", group.StandardId);
      ViewData["SubjectId"] = new SelectList(_context.Subjects.OrderBy(b => b.Name), "Id", "Name", group.SubjectId);
      return View(group);
    }

    // POST: Classes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BoardId,StandardId,SubjectId")] Classroom group)
    {
      if (id != group.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(group);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!GroupExists(group.Id))
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
      ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", group.BoardId);
      ViewData["StandardId"] = new SelectList(_context.Standards, "Id", "Id", group.StandardId);
      ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", group.SubjectId);
      return View(group);
    }

    // GET: Classes/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var group = await _context.Classes
          .Include(g => g.Board)
          .Include(g => g.Standard)
          .Include(g => g.Subject)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (group == null)
      {
        return NotFound();
      }

      return View(group);
    }

    // POST: Classes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var group = await _context.Classes.Include(g => g.Users).FirstOrDefaultAsync(g => g.Id == id);

      _context.Classes.Remove(group);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool GroupExists(int id)
    {
      return _context.Classes.Any(e => e.Id == id);
    }
  }
}
