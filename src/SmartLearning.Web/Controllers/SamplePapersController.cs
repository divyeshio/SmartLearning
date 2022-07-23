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
    public class SamplePapersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _env;
        private UserManager<ApplicationUser> _userManager;

        public SamplePapersController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = webHostEnvironment;
            _userManager = userManager;
        }

        // GET: SamplePapers
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> List(long? subject, long? board, int? standard)
        {
            if (HttpContext.User.IsInRole("Admin"))
            {
                ViewData["Boards"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name", board);
                ViewData["Standards"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Name", standard);
                ViewData["Subjects"] = new SelectList(_context.Subjects.OrderBy(b => b.Name), "Id", "Name", subject);
                var chapters = from s in _context.SamplePapers
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
                return View(await chapters.Include(c => c.Class.Board).Include(c => c.Class.Standard).Include(c => c.Class.Subject).Include(c => c.UploadedBy).ToListAsync());
            }
            else
                return View(await _context.SamplePapers.Include(c => c.UploadedBy).Include(c => c.Class.Board).Include(c => c.Class.Standard).Include(c => c.Class.Subject).Where(c => c.Class.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.Class.BoardId == int.Parse(User.FindFirst("BoardId").Value) && c.Class.SubjectId == int.Parse(HttpContext.User.FindFirst("SubjectId").Value)).ToListAsync());
        }

        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Student"))
                return View(await _context.Classes.Where(c => c.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.BoardId == int.Parse(User.FindFirst("BoardId").Value)).Include(c => c.Subject).AsNoTracking().ToListAsync());
            return View(await _context.Classes.AsNoTracking().Include(c => c.Subject).ToListAsync());
        }

        public async Task<IActionResult> ViewSamplePaper(int id)
        {
            if (id == null) return NotFound();
            ViewData["ClassName"] = await _context.Classes.Where(c => c.Id == id).Select(c => c.Name).FirstOrDefaultAsync();
            return View(await _context.SamplePapers.Where(c => c.ClassId == id).Include(c => c.Class).AsNoTracking().ToListAsync());
        }


        // GET: SamplePapers/Create
        [Authorize(Roles = "Admin")]
        [Route("SamplePapers/Add/Admin", Name = "AddSamplePaper")]
        [HttpGet]
        public async Task<IActionResult> AddSamplePaper()
        {
            return View(await populateSBS(new SamplePaperViewModel()));
        }

        // POST: SamplePapers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("SamplePapers/Add/Admin", Name = "AddSamplePaper")]
        public async Task<IActionResult> AddSamplePaper(SamplePaperViewModel vm)
        {
            if (vm.File == null)
            {
                ModelState.AddModelError(string.Empty, "Please attach a file");
                vm = await populateSBS(vm);
                vm.Classes = await getClasses1(vm.ClassId);
                return View(vm);
            }
            if (ModelState.IsValid)
            {
                var samplepaper = new SamplePaper();
                samplepaper.Year = vm.Year;
                samplepaper.FileName = vm.File.FileName;
                samplepaper.ClassId = vm.ClassId;
                samplepaper.SamplePaperUrl = await UploadedFile(vm.File);
                samplepaper.UploadedBy = await _userManager.GetUserAsync(User);
                await _context.SamplePapers.AddAsync(samplepaper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            vm = await populateSBS(vm);
            vm.Classes = await getClasses1(vm.ClassId);
            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Faculty")]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> Add([Bind("Year,File")] SamplePaperViewModel vm)
        {
            if (vm.File == null)
            {
                ModelState.AddModelError(string.Empty, "Please attach a file");
                return View(vm);
            }
            if (ModelState.IsValid)
            {
                var samplepaper = new SamplePaper();
                samplepaper.FileName = vm.File.FileName;
                samplepaper.Year = vm.Year;
                samplepaper.ClassId = await _context.Classes.Where(c => c.StandardId == int.Parse(User.FindFirst("StandardId").Value) && c.BoardId == int.Parse(User.FindFirst("BoardId").Value) && c.SubjectId == int.Parse(User.FindFirst("SubjectId").Value)).Select(c => c.Id).FirstOrDefaultAsync();
                samplepaper.SamplePaperUrl = await UploadedFile(vm.File);
                samplepaper.UploadedBy = await _userManager.GetUserAsync(User);
                await _context.SamplePapers.AddAsync(samplepaper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Faculty,Student")]
        public async Task<FileResult> Download(long? id)
        {
            if (id == null) return File("", "");

            var note = await _context.SamplePapers.FindAsync(id);
            var path = Path.Combine(_env.ContentRootPath, "Storage", "SamplePapers", note.SamplePaperUrl);
            var fileBytes = System.IO.File.ReadAllBytes(path);
            var fileName = note.SamplePaperUrl.Split("_")[1];
            return File(fileBytes, "application/octet-stream", fileName);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Faculty,Student")]
        public async Task<FileResult> ViewPdf(long? id)
        {
            if (id == null) return File("", "");

            var note = await _context.SamplePapers.FindAsync(id);
            var path = Path.Combine(_env.ContentRootPath, "Storage", "SamplePapers", note.SamplePaperUrl);
            var ms = new FileStream(path, FileMode.Open);
            return File(ms, "application/pdf");
        }

        // GET: SamplePapers/Edit/5
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.SamplePapers.Include(s => s.UploadedBy).Include(s => s.Class).AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (note == null)
            {
                return NotFound();
            }
            return View(note);
        }

        // POST: SamplePapers/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,Faculty")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Year,File")] SamplePaperViewModel samplepaper)
        {
            if (id != samplepaper.Id)
            {
                return NotFound();
            }
            var actualSamplePaper = await _context.SamplePapers.FindAsync(samplepaper.Id);
            if (actualSamplePaper == null)
            {
                return NotFound();
            }
            if (samplepaper.File != null)
            {
                if (await RemoveFile(actualSamplePaper.SamplePaperUrl))
                {
                    actualSamplePaper.FileName = Path.GetFileName(samplepaper.File.FileName);
                    actualSamplePaper.SamplePaperUrl = await UploadedFile(samplepaper.File);
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (actualSamplePaper.Year != samplepaper.Year)
                        actualSamplePaper.Year = samplepaper.Year;
                    _context.SamplePapers.Update(actualSamplePaper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SamplePaperExists(samplepaper.Id))
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
            return View(samplepaper);
        }


        // POST: SamplePapers/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin,Faculty")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var note = await _context.SamplePapers.FindAsync(id);
            if (await RemoveFile(note.SamplePaperUrl))
            {
                _context.SamplePapers.Remove(note);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else return BadRequest();
        }
        private Task<bool> RemoveFile(string filename)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, "Storage", "SamplePapers", filename);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }


        /*  [Authorize(Roles = "Admin")]
          [HttpPost]
          public async Task<JsonResult> getClasses([FromBody] GetChaptersViewModel model)
          {
            if (ModelState.IsValid)
            {
              var classa = await _context.Classes.Where(c => c.StandardId == model.Standard && c.BoardId == model.Board && c.SubjectId == model.Subject).Select(c => new Classroom { Id = c.Id, Name = c.Name }).AsNoTracking().SingleAsync();
              return Json(classa);
            }
            return Json(new { });
          }*/


        private Task<string> UploadedFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                var uploadsFolder = Path.Combine(_env.ContentRootPath, "Storage", "SamplePapers");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return Task.FromResult(uniqueFileName);
        }
        private bool SamplePaperExists(int id)
        {
            return _context.SamplePapers.Any(e => e.Id == id);
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
        public async Task<SelectList> getClasses1(int? classId = null)
        {
            return new SelectList(await _context.Classes.OrderBy(b => b.Name).AsNoTracking().ToListAsync(), "Id", "Name", classId);
        }

        public async Task<SamplePaperViewModel> populateSBS(SamplePaperViewModel model, int? standardId = null, int? boardId = null, int? subjectId = null)
        {
            model.Standards = await getStandards(standardId);
            model.Subjects = await getSubjects(subjectId);
            model.Boards = await getBoards(boardId);
            return model;
        }
    }
}
