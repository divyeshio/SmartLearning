using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Infrastructure.Data;

namespace SmartLearning.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BoardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*   // GET: Boards
           public async Task<IActionResult> Index()
           {
             return View(await _context.Boards.AsNoTracking().ToListAsync());
           }*/


        // GET: Boards/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: Boards/Create
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("Id,Name")] Board board)
        {
          if (ModelState.IsValid)
          {
            board.AbbrName = board.AbbrName.ToUpper();
            _context.Add(board);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
          }
          return View(board);
        }*/

        // GET: Boards/Edit/5
        /*    public async Task<IActionResult> Edit(long? id)
            {
              if (id == null)
              {
                return NotFound();
              }

              var board = await _context.Boards.FirstOrDefaultAsync(b => b.Id == id);
              if (board == null)
              {
                return NotFound();
              }
              return View(board);
            }*/

        // POST: Boards/Edit/5
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name")] Board board)
        {
          if (id != board.Id)
          {
            return NotFound();
          }

          if (ModelState.IsValid)
          {
            try
            {
              board.AbbrName = board.AbbrName.ToUpper();
              _context.Update(board);
              await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
              if (!BoardExists(board.Id))
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
          return View(board);
        }*/


        // POST: Boards/Delete/5
        /*    [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(long id)
            {
              var board = await _context.Boards.Include(b => b.Users).Where(b => b.Id == id).FirstAsync();
              //_context.Classes.FromSqlRaw("");
              _context.Boards.Remove(board);
              await _context.SaveChangesAsync();
              return RedirectToAction(nameof(Index));
            }

            private bool BoardExists(long id)
            {
              return _context.Boards.Any(e => e.Id == id);
            }*/
    }
}
