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
    public class ReferenceBooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _env;
        private UserManager<ApplicationUser> _userManager;

        public ReferenceBooksController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
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
                var referenceBooks = from s in _context.ReferenceBooks
                                     select s;
                if (subject != null)
                {
                    referenceBooks = referenceBooks.Where(s => s.Class.SubjectId == subject);
                }
                if (board != null)
                {
                    referenceBooks = referenceBooks.Where(c => c.Class.BoardId == board);
                }
                if (standard != null)
                {
                    referenceBooks = referenceBooks.Where(c => c.Class.StandardId == standard);
                }
                return View(await referenceBooks.Include(c => c.Class.Board).Include(c => c.Class.Standard).Include(c => c.Class.Subject).ToListAsync());
            }
            else
                return View(await _context.ReferenceBooks.Include(c => c.Class).Include(c => c.Class.Board).Include(c => c.Class.Standard).Include(c => c.Class.Subject).Where(c => c.Class.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.Class.BoardId == int.Parse(User.FindFirst("BoardId").Value) && c.Class.SubjectId == int.Parse(HttpContext.User.FindFirst("SubjectId").Value)).ToListAsync());
        }

        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Student"))
                return View(await _context.Classes.Where(c => c.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.BoardId == int.Parse(User.FindFirst("BoardId").Value)).Include(c => c.Subject).AsNoTracking().ToListAsync());
            return View(await _context.Classes.AsNoTracking().Include(c => c.Subject).ToListAsync());
        }

        public async Task<IActionResult> ViewReferenceBooks(int id)
        {
            if (id == null) return NotFound();
            ViewData["ClassName"] = await _context.Classes.Where(c => c.Id == id).Select(c => c.Name).AsNoTracking().FirstOrDefaultAsync();
            return View(await _context.ReferenceBooks.Where(c => c.ClassId == id).Include(c => c.Class).AsNoTracking().ToListAsync());
        }


        // GET: SamplePapers/Create
        [Authorize(Roles = "Admin")]
        [Route("ReferenceBooks/Add/Admin", Name = "AddReferenceBook")]
        [HttpGet]
        public async Task<IActionResult> AddReferenceBook()
        {
            return View(await populateSBS(new ReferenceBookViewModel()));
        }

        // POST: SamplePapers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("ReferenceBooks/Add/Admin", Name = "AddReferenceBook")]
        public async Task<IActionResult> AddReferenceBook(ReferenceBookViewModel vm)
        {
            if (vm.File == null || vm.ImageFile == null)
            {
                ModelState.AddModelError(string.Empty, "Please attach a file");
                vm = await populateSBS(vm);
                vm.Classes = await getClasses1(vm.ClassId);
                return View(vm);
            }
            if (ModelState.IsValid)
            {
                var referenceBook = new ReferenceBook
                {
                    BookName = vm.BookName,
                    ClassId = vm.ClassId,
                    FileUrl = await UploadedFile(vm.File),
                    FileName = Path.GetFileNameWithoutExtension(vm.File.FileName),
                    ImageName = await UploadedImage(vm.ImageFile),
                    AuthorName = vm.AuthorName,
                    Price = vm.Price
                };

                await _context.ReferenceBooks.AddAsync(referenceBook);
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
            return View(new ReferenceBookViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> Add(ReferenceBookViewModel vm)
        {
            if (vm.File == null || vm.ImageFile == null)
            {
                ModelState.AddModelError(string.Empty, "Please attach a file");
                return View(vm);
            }
            ModelState.Remove("ClassId");
            if (ModelState.IsValid)
            {
                var referenceBook = new ReferenceBook
                {
                    BookName = vm.BookName,
                    ClassId = await _context.Classes.Where(c => c.StandardId == int.Parse(User.FindFirst("StandardId").Value) && c.BoardId == int.Parse(User.FindFirst("BoardId").Value) && c.SubjectId == int.Parse(User.FindFirst("SubjectId").Value)).Select(c => c.Id).FirstOrDefaultAsync(),
                    FileUrl = await UploadedFile(vm.File),
                    FileName = Path.GetFileNameWithoutExtension(vm.File.FileName),
                    ImageName = await UploadedImage(vm.ImageFile),
                    AuthorName = vm.AuthorName,
                    Price = vm.Price
                };

                await _context.ReferenceBooks.AddAsync(referenceBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(vm);
        }

        private Task<string> UploadedImage(IFormFile imageFile)
        {
            string uniqueFileName = null;

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "StaticFiles", "ReferenceBooks");
            uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }
            return Task.FromResult(uniqueFileName);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Faculty,Student")]
        public async Task<FileResult> Download(long? id)
        {
            if (id == null) return File("", "");

            var note = await _context.ReferenceBooks.FindAsync(id);
            var path = Path.Combine(_env.ContentRootPath, "Storage", "ReferenceBooks", note.FileUrl);
            var fileBytes = System.IO.File.ReadAllBytes(path);
            var fileName = note.FileUrl.Split("_")[1];
            return File(fileBytes, "application/octet-stream", fileName);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Faculty,Student")]
        public async Task<FileResult> ViewPdf(long? id)
        {
            if (id == null) return File("", "");

            var note = await _context.ReferenceBooks.FindAsync(id);
            var path = Path.Combine(_env.ContentRootPath, "Storage", "ReferenceBooks", note.FileUrl);
            var ms = new FileStream(path, FileMode.Open);
            return File(ms, "application/pdf");
        }

        // GET: SamplePapers/Edit/5
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.ReferenceBooks.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            var vm = new ReferenceBookViewModel
            {
                AuthorName = book.AuthorName,
                BookName = book.BookName,
                Price = book.Price,
                ImageName = book.ImageName,
                FileName = book.FileName,
                ClassId = book.ClassId
            };
            return View(vm);
        }

        // POST: SamplePapers/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,Faculty")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ReferenceBookViewModel referenceBook)
        {
            if (id != referenceBook.Id)
            {
                return NotFound();
            }
            var actualReferenceBook = await _context.ReferenceBooks.FindAsync(referenceBook.Id);
            if (actualReferenceBook == null)
            {
                return NotFound();
            }
            ModelState.Remove("File");
            ModelState.Remove("ImageFile");
            if (ModelState.IsValid)
            {
                if (referenceBook.File != null)
                {
                    if (await RemoveFile(actualReferenceBook.FileUrl))
                    {
                        actualReferenceBook.FileName = Path.GetFileNameWithoutExtension(referenceBook.File.FileName);
                        actualReferenceBook.FileUrl = await UploadedFile(referenceBook.File);
                    }
                }
                if (referenceBook.ImageFile != null)
                {
                    if (await RemoveImage(actualReferenceBook.ImageName))
                    {
                        actualReferenceBook.ImageName = await UploadedImage(referenceBook.ImageFile);
                    }
                }
                try
                {
                    if (actualReferenceBook.BookName != referenceBook.BookName)
                        actualReferenceBook.BookName = referenceBook.BookName;
                    if (actualReferenceBook.Price != referenceBook.Price)
                        actualReferenceBook.Price = referenceBook.Price;
                    if (actualReferenceBook.AuthorName != referenceBook.AuthorName)
                        actualReferenceBook.AuthorName = referenceBook.BookName;
                    _context.ReferenceBooks.Update(actualReferenceBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SamplePaperExists(referenceBook.Id))
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
            return View(referenceBook);
        }


        // POST: SamplePapers/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin,Faculty")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var book = await _context.ReferenceBooks.FindAsync(id);
            if (await RemoveFile(book.FileUrl) && await RemoveImage(book.ImageName))
            {
                _context.ReferenceBooks.Remove(book);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else return BadRequest();
        }
        private Task<bool> RemoveFile(string filename)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, "Storage", "ReferenceBooks", filename);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        private Task<bool> RemoveImage(string filename)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, "StaticFiles", "ReferenceBooks", filename);
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
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "Storage", "ReferenceBooks");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
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

        public async Task<ReferenceBookViewModel> populateSBS(ReferenceBookViewModel model, int? standardId = null, int? boardId = null, int? subjectId = null)
        {
            model.Standards = await getStandards(standardId);
            model.Subjects = await getSubjects(subjectId);
            model.Boards = await getBoards(boardId);
            return model;
        }
    }
}
