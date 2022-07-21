using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Controllers
{
  [Authorize]
  public class NotesController : Controller
  {
    private readonly ApplicationDbContext _context;
    private IWebHostEnvironment _env;
    private UserManager<ApplicationUser> _userManager;

    public NotesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _env = webHostEnvironment;
      _userManager = userManager;
    }

    // GET: Notes
    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> List(long? subject, long? board, int? standard)
    {
      if (HttpContext.User.IsInRole("Admin"))
      {
        ViewData["Boards"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name", board);
        ViewData["Standards"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Name", standard);
        ViewData["Subjects"] = new SelectList(_context.Subjects.OrderBy(b => b.Name), "Id", "Name", subject);
        var chapters = from s in _context.Notes
                       select s;
        if (subject != null)
        {
          chapters = chapters.Where(s => s.Chapter.Class.SubjectId == subject);
        }
        if (board != null)
        {
          chapters = chapters.Where(c => c.Chapter.Class.BoardId == board);
        }
        if (standard != null)
        {
          chapters = chapters.Where(c => c.Chapter.Class.StandardId == standard);
        }
        return View(await chapters.Include(c => c.Chapter.Class.Board).Include(c => c.Chapter.Class.Standard).Include(c => c.Chapter.Class.Subject).Include(c => c.UploadedBy).ToListAsync());
      }
      else
        return View(await _context.Notes.Include(c => c.UploadedBy).Include(c => c.Chapter.Class.Board).Include(c => c.Chapter.Class.Standard).Include(c => c.Chapter.Class.Subject).Where(c => c.Chapter.Class.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.Chapter.Class.BoardId == int.Parse(User.FindFirst("BoardId").Value) && c.Chapter.Class.SubjectId == int.Parse(HttpContext.User.FindFirst("SubjectId").Value)).ToListAsync());
    }

    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> Index()
    {
      if (User.IsInRole("Student"))
        return View(await _context.Classes.Where(c => c.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.BoardId == int.Parse(User.FindFirst("BoardId").Value)).Include(c => c.Subject).AsNoTracking().ToListAsync());
      return View(await _context.Classes.AsNoTracking().Include(c => c.Subject).ToListAsync());
    }

    public async Task<IActionResult> ViewNotes(int id)
    {
      if (id == null) return NotFound();
      ViewData["ClassName"] = await _context.Classes.Where(c => c.Id == id).Select(c => c.Name).AsNoTracking().FirstOrDefaultAsync();
      ViewData["SubjectName"] = await _context.Classes.Where(c => c.Id == id).Include(classa => classa.Subject).Select(c => c.Subject.Name).AsNoTracking().FirstOrDefaultAsync();
      return View(await _context.Notes.Where(c => c.Chapter.ClassId == id).Include(c => c.Chapter).ThenInclude(c => c.Class).AsNoTracking().ToListAsync());
    }

    // GET: Notes/Details/5
    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> Details(long? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var note = await _context.Notes
          .AsNoTracking()
          .FirstOrDefaultAsync(m => m.Id == id);
      if (note == null)
      {
        return NotFound();
      }

      return View(note);
    }

    // GET: Notes/Create
    [Authorize(Roles = "Admin")]
    [Route("Notes/Add/Admin", Name = "AddNote")]
    [HttpGet]
    public async Task<IActionResult> AddNote()
    {
      return View(await populateSBS(new NoteViewModel()));
    }

    // POST: Notes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    [Route("Notes/Add/Admin", Name = "AddNote")]
    public async Task<IActionResult> AddNote([Bind("ChapterId,NoteFile")] NoteViewModel noteVM)
    {
      if (noteVM.NoteFile == null)
      {
        ModelState.AddModelError(string.Empty, "Please attach a file");
        return View(noteVM);
      }
      if (ModelState.IsValid)
      {
        var note = new Note();
        note.FileName = noteVM.NoteFile.FileName;
        note.ChapterId = noteVM.ChapterId;
        note.NoteUrl = await UploadedFile(noteVM.NoteFile);
        note.UploadedBy = await _userManager.GetUserAsync(User);
        await _context.Notes.AddAsync(note);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(List));
      }
      noteVM = await populateSBS(noteVM);
      noteVM.Chapters = await getChapters(noteVM.ChapterId);
      return View(noteVM);
    }

    [HttpGet]
    [Authorize(Roles = "Faculty")]
    public async Task<IActionResult> Add()
    {
      var chapters = new SelectList(await _context.Chapters.Where(c => c.Class.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.Class.BoardId == int.Parse(User.FindFirst("BoardId").Value) && c.Class.SubjectId == int.Parse(HttpContext.User.FindFirst("SubjectId").Value)).ToListAsync(), "Id", "Name");
      return View(new NoteViewModel { Chapters = chapters });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Faculty")]
    public async Task<IActionResult> Add([Bind("ChapterId,NoteFile")] NoteViewModel noteVM)
    {
      ModelState.Remove("Id");
      if (noteVM.NoteFile == null)
      {
        ModelState.AddModelError(string.Empty, "Please attach a file");
        noteVM.Chapters = new SelectList(await _context.Chapters.Where(c => c.Class.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.Class.BoardId == int.Parse(User.FindFirst("BoardId").Value) && c.Class.SubjectId == int.Parse(HttpContext.User.FindFirst("SubjectId").Value)).ToListAsync(), "Id", "Name");
        return View(noteVM);
      }
      if (ModelState.IsValid)
      {
        var note = new Note();
        note.FileName = noteVM.NoteFile.FileName;
        note.ChapterId = noteVM.ChapterId;
        note.NoteUrl = await UploadedFile(noteVM.NoteFile);
        note.UploadedBy = await _userManager.GetUserAsync(User);
        await _context.Notes.AddAsync(note);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(List));
      }
      noteVM.Chapters = new SelectList(await _context.Chapters.Where(c => c.Class.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.Class.BoardId == int.Parse(User.FindFirst("BoardId").Value) && c.Class.SubjectId == int.Parse(HttpContext.User.FindFirst("SubjectId").Value)).ToListAsync(), "Id", "Name");
      return View(noteVM);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Faculty,Student")]
    public async Task<FileResult> Download(long? id)
    {
      if (id == null) return File("", "");

      var note = await _context.Notes.FindAsync(id);
      var path = Path.Combine(_env.ContentRootPath, "Storage", "Notes", note.NoteUrl);
      var fileBytes = System.IO.File.ReadAllBytes(path);
      var fileName = note.NoteUrl.Split("_")[1];
      return File(fileBytes, "application/octet-stream", fileName);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Faculty,Student")]
    public async Task<FileResult> ViewPdf(long? id)
    {
      if (id == null) return File("", "");

      var note = await _context.Notes.FindAsync(id);
      var path = Path.Combine(_env.ContentRootPath, "Storage", "Notes", note.NoteUrl);
      var ms = new FileStream(path, FileMode.Open);
      return File(ms, "application/pdf");
    }

    // GET: Notes/Edit/5
    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> Edit(long id)
    {
      var note = await _context.Notes.Include(c => c.UploadedBy).Where(n => n.Id == id).Include(c => c.Chapter).AsNoTracking().FirstOrDefaultAsync();
      if (note == null)
      {
        return NotFound();
      }
      ViewData["Chapters"] = new SelectList(await _context.Chapters.OrderBy(b => b.Name).Where(c => c.ClassId == note.Chapter.ClassId).Include(c => c.Class).AsNoTracking().ToListAsync(), "Id", "Name", note.ChapterId);
      return View(note);
    }
    // POST: Notes/Edit/5
    [HttpPost]
    [Authorize(Roles = "Admin,Faculty")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, [Bind("Id,ChapterId,NoteFile")] NoteViewModel noteVM)
    {
      if (id != noteVM.Id)
      {
        return NotFound();
      }

      var note = await _context.Notes.FindAsync(noteVM.Id);
      var temp = await _context.Chapters.Where(c => c.Id == noteVM.ChapterId).FirstOrDefaultAsync();
      if (temp == null)
        return NotFound();
      if (note == null)
      {
        return NotFound();
      }
      if (noteVM.NoteFile != null)
      {
        if (await RemoveFile(note.NoteUrl))
        {
          note.FileName = Path.GetFileName(noteVM.NoteFile.FileName);
          note.NoteUrl = await UploadedFile(noteVM.NoteFile);
        }
      }
      ModelState.Remove("Chapter");
      if (ModelState.IsValid)
      {
        try
        {
          if (note.ChapterId != noteVM.ChapterId)
          {
            note.ChapterId = temp.Id;
          }
          _context.Notes.Update(note);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!NoteExists(noteVM.Id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(List));
      }
      return View(noteVM);
    }




    // POST: Notes/Delete/5
    [HttpPost]
    [Authorize(Roles = "Admin,Faculty")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
      var note = await _context.Notes.FindAsync(id);
      if (await RemoveFile(note.NoteUrl))
      {
        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
        return Ok();
      }
      else return BadRequest();
    }
    private Task<bool> RemoveFile(string filename)
    {
      var fullPath = Path.Combine(_env.ContentRootPath, "Storage", "Notes", filename);
      if (System.IO.File.Exists(fullPath))
      {
        System.IO.File.Delete(fullPath);
        return Task.FromResult(true);
      }
      return Task.FromResult(false);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<JsonResult> getChaptersApi([FromBody] GetChaptersViewModel model)
    {
      if (ModelState.IsValid)
      {
        var classa = await _context.Classes.AsNoTracking().Where(c => c.StandardId == model.Standard && c.BoardId == model.Board && c.SubjectId == model.Subject).Select(c => c.Id).SingleAsync();
        var data = await _context.Chapters.Where(c => c.ClassId == classa).Select(c => new Chapter { Id = c.Id, Name = c.Name, SerialNo = c.SerialNo }).OrderBy(c => c.SerialNo).ToListAsync();
        return Json(data);
      }
      return Json(new { });
    }


    private Task<string> UploadedFile(IFormFile file)
    {
      string uniqueFileName = null;

      if (file != null)
      {
        var uploadsFolder = Path.Combine(_env.ContentRootPath, "Storage", "Notes");
        uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          file.CopyTo(fileStream);
        }
      }
      return Task.FromResult(uniqueFileName);
    }
    private bool NoteExists(long id)
    {
      return _context.Notes.Any(e => e.Id == id);
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

    public async Task<NoteViewModel> populateSBS(NoteViewModel model, int? standardId = null, int? boardId = null, int? subjectId = null)
    {
      model.Standards = await getStandards(standardId);
      model.Subjects = await getSubjects(subjectId);
      model.Boards = await getBoards(boardId);
      return model;
    }
  }
}
