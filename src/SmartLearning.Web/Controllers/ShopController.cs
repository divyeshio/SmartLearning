using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Infrastructure.Data;

namespace SmartLearning.Web.Controllers
{
  public class ShopController : Controller
  {
    private readonly ApplicationDbContext _context;

    public ShopController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: Shop
    public async Task<IActionResult> Index()
    {
      return View(await _context.Books.ToListAsync());
    }

    // GET: Shop/Details/5
    public async Task<IActionResult> Details(long? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var book = await _context.Books
          .FirstOrDefaultAsync(m => m.Id == id);
      if (book == null)
      {
        return NotFound();
      }

      return View(book);
    }

    // GET: Shop/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Shop/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Price,AuthorName,PublisherName,Year,Details,ImageUrl")] Book book)
    {
      if (ModelState.IsValid)
      {
        _context.Add(book);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(book);
    }

    // GET: Shop/Edit/5
    public async Task<IActionResult> Edit(long? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var book = await _context.Books.FindAsync(id);
      if (book == null)
      {
        return NotFound();
      }
      return View(book);
    }

    // POST: Shop/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, [Bind("Id,Title,Price,AuthorName,PublisherName,Year,Details,ImageUrl")] Book book)
    {
      if (id != book.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(book);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!BookExists(book.Id))
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
      return View(book);
    }

    // POST: Shop/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
      var book = await _context.Books.FindAsync(id);
      _context.Books.Remove(book);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool BookExists(long id)
    {
      return _context.Books.Any(e => e.Id == id);
    }
  }
}
