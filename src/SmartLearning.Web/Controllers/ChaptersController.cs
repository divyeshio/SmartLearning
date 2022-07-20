using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty")]
    public class ChaptersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ChaptersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Chapters
        public async Task<IActionResult> Index(long? subject, long? board, int? standard)
        {
            if (HttpContext.User.IsInRole("Admin"))
            {
                ViewData["Boards"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name", board);
                ViewData["Standards"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Name", standard);
                ViewData["Subjects"] = new SelectList(_context.Subjects.OrderBy(b => b.Name), "Id", "Name", subject);
                var chapters = from s in _context.Chapters
                               select s;
                if (subject != null)
                {
                    chapters = chapters.Where(s => s.Class.SubjectId == subject);
                }
                if (board != null)
                {
                    chapters = chapters.Where(c => c.Class.BoardId == board);
                }
                if (standard != null)
                {
                    chapters = chapters.Where(c => c.Class.StandardId == standard);
                }
                return View(await chapters.Include(c => c.Class.Board).Include(c => c.Class.Standard).Include(c => c.Class.Subject).ToListAsync());
            }
            else
                return View(await _context.Chapters.Include(c => c.Class.Board).Include(c => c.Class.Standard).Include(c => c.Class.Subject).Where(c => c.Class.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.Class.BoardId == int.Parse(User.FindFirst("BoardId").Value) && c.Class.SubjectId == int.Parse(HttpContext.User.FindFirst("SubjectId").Value)).ToListAsync());
        }

        // GET: Chapters/Create
        /* [Route("/Chapters/Add/Admin", Name = "AddChapter")]
         [Authorize(Roles = "Admin")]
         [HttpGet]
         public async Task<IActionResult> AddChapter()
         {
             return View(new ChapterViewModel { Boards = await getBoards(), Standards = await getStandards(), Subjects = await getSubjects() });
         }

         [HttpPost]
         [Route("Chapters/Add/Admin", Name = "AddChapter")]
         [Authorize(Roles = "Admin")]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> AddChapter([Bind("SerialNo,Name,BoardId,StandardId,SubjectId")] ChapterViewModel chapter)
         {
             var classa = await _context.Classes.Where(c => c.BoardId == chapter.BoardId && c.StandardId == chapter.StandardId && c.SubjectId == chapter.SubjectId).FirstOrDefaultAsync();
             if (classa == null)
             {
                 ModelState.AddModelError(string.Empty, "Please create a Class first");
                 chapter.Boards = await getBoards();
                 chapter.Standards = await getStandards();
                 chapter.Subjects = await getSubjects();
                 return View(chapter);
             }
             if (ModelState.IsValid)
             {
                 await _context.Chapters.AddAsync(new Chapter { Class = classa, Name = chapter.Name, SerialNo = chapter.SerialNo });
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(Index));
             }
             return View(chapter);
         }
    */
        public IActionResult Add()
        {
            return View();
        }

        // POST: Chapters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("SerialNo,Name,BoardId,StandardId,SubjectId")] ChapterViewModel chapter)
        {
            var classa = await _context.Classes.Where(c => c.BoardId == chapter.BoardId && c.StandardId == chapter.StandardId && c.SubjectId == chapter.SubjectId).FirstOrDefaultAsync();
            if (classa == null)
            {
                ModelState.AddModelError(string.Empty, "Please create a Class first");
                chapter.Boards = await getBoards();
                chapter.Standards = await getStandards();
                chapter.Subjects = await getSubjects();
                return View("AdminAdd", chapter);
            }
            if (ModelState.IsValid)
            {
                await _context.Chapters.AddAsync(new Chapter { Class = classa, Name = chapter.Name, SerialNo = chapter.SerialNo });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chapter);
        }



        // GET: Chapters/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters.Include(c => c.Class).ThenInclude(c => c.Board).Include(c => c.Class.Subject).Include(c => c.Class.Standard).FirstOrDefaultAsync(c => c.Id == id);
            if (chapter == null)
            {
                return NotFound();
            }
            return View(new ChapterViewModel { Name = chapter.Name, SerialNo = chapter.SerialNo, StandardId = chapter.Class.StandardId, SubjectId = chapter.Class.SubjectId, BoardId = chapter.Class.BoardId });
        }

        [Authorize(Roles = "Admin")]
        [Route("Chapters/Edit/Admin/{id}", Name = "EditChapter")]
        [HttpGet]
        public async Task<IActionResult> EditChapter(long id)
        {
            var chapter = await _context.Chapters.Where(c => c.Id == id).Include(c => c.Class).ThenInclude(c => c.Board).Include(c => c.Class.Standard).Include(c => c.Class.Subject).AsNoTracking().FirstOrDefaultAsync();
            if (chapter == null)
            {
                return NotFound();
            }
            var cm = new ChapterViewModel { Boards = await getBoards(chapter.Class.BoardId), Standards = await getStandards(chapter.Class.StandardId), Subjects = await getSubjects(chapter.Class.SubjectId), Name = chapter.Name, SerialNo = chapter.SerialNo };
            return View(cm);
        }

        // POST: Chapters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,SerialNo,Name,BoardId,StandardId,SubjectId")] ChapterViewModel chapterVM)
        {
            if (id != chapterVM.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var chapter = await _context.Chapters.FindAsync(chapterVM.Id);
                var temp = await _context.Classes.Where(c => c.StandardId == chapterVM.StandardId && c.BoardId == chapterVM.BoardId && c.SubjectId == chapterVM.SubjectId).FirstOrDefaultAsync();
                if (temp == null)
                    return NotFound();
                try
                {
                    if (chapter.Name != chapterVM.Name)
                    {
                        chapter.Name = chapterVM.Name;
                    }
                    if (chapter.SerialNo != chapterVM.SerialNo)
                    {
                        chapter.SerialNo = chapterVM.SerialNo;
                    }
                    if (chapter.ClassId != temp.Id)
                    {
                        chapter.ClassId = temp.Id;
                    }
                    _context.Chapters.Update(chapter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChapterExists(chapter.Id))
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
            return View(chapterVM);
        }

        // POST: Chapters/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool ChapterExists(long id)
        {
            return _context.Chapters.Any(e => e.Id == id);
        }

        public async Task<SelectList> getBoards(int? boardId = null)
        {
            return new SelectList(await _context.Boards.OrderBy(b => b.AbbrName).AsNoTracking().ToListAsync(), "Id", "Name", boardId);
        }
        public async Task<SelectList> getSubjects(int? subjectId = null)
        {
            return new SelectList(await _context.Subjects.OrderBy(b => b.Name).AsNoTracking().ToListAsync(), "Id", "Name", subjectId);
        }
        public async Task<SelectList> getStandards(int? standardId = null)
        {
            return new SelectList(await _context.Standards.OrderBy(b => b.Level).AsNoTracking().ToListAsync(), "Id", "Name", standardId);
        }
        public async Task<SelectList> getChapters(int? chapterId = null)
        {
            return new SelectList(await _context.Chapters.OrderBy(b => b.Name).AsNoTracking().ToListAsync(), "Id", "Name", chapterId);
        }
    }
}
