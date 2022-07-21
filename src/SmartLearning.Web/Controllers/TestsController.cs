using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.TestAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IMapper _mapper;
        public TestsController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> List(long? subject, long? board, int? standard)
        {
            if (User.IsInRole("Admin"))
            {
                ViewData["Boards"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name", board);
                ViewData["Standards"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Name", standard);
                ViewData["Subjects"] = new SelectList(_context.Subjects.OrderBy(b => b.Name).ToList(), "Id", "Name", subject);
                var tests = from s in _context.Tests
                            select s;
                if (subject != null)
                {
                    tests = tests.Where(s => s.Chapter.Class.SubjectId == subject);
                }
                if (board != null)
                {
                    tests = tests.Where(c => c.Chapter.Class.BoardId == board);
                }
                if (standard != null)
                {
                    tests = tests.Where(c => c.Chapter.Class.StandardId == standard);
                }
                return View(await _context.Tests.Include(t => t.Chapter).ThenInclude(c => c.Class).ThenInclude(c => c.Standard).Include(c => c.Chapter.Class.Subject).Include(c => c.Chapter.Class.Board).Include(t => t.CreatedBy).ToListAsync());
            }
            else if (User.IsInRole("Faculty"))
            {
                return View(await _context.Tests.Include(t => t.Chapter).ThenInclude(c => c.Class).ThenInclude(c => c.Standard).Include(c => c.Chapter.Class.Subject).Include(c => c.Chapter.Class.Board).Include(t => t.CreatedBy).ToListAsync());

            }
            return NotFound();
        }

        // GET: Tests
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Student"))
                return View(await _context.Classes.Where(c => c.StandardId == int.Parse(HttpContext.User.FindFirst("StandardId").Value) && c.BoardId == int.Parse(User.FindFirst("BoardId").Value)).Include(c => c.Subject).AsNoTracking().ToListAsync());
            return View(await _context.Classes.AsNoTracking().Include(c => c.Subject).ToListAsync());
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SelectChapter(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var classa = await _context.Classes.Where(c => c.Id == id).AsNoTracking().FirstOrDefaultAsync();
            if (classa == null)
                return NotFound();
            var tests = await _context.Tests.Where(c => c.Chapter.ClassId == classa.Id).AsNoTracking().Include(c => c.Chapter).AsNoTracking().ToListAsync();
            var user = await _userManager.GetUserAsync(User);
            var testAttempts = await _context.TestAttempts.Where(t => t.StudentId == user.Id).Include(t => t.Test).ThenInclude(t => t.Chapter).Include(t => t.Test.Questions).AsNoTracking().ToListAsync();
            var listOfTest = new List<UserTest>();
            foreach (var item in testAttempts)
            {
                if (item.isCompleted)
                {
                    listOfTest.Add(new UserTest { isAttempted = true, isCompleted = true, TestResult = await _context.TestResults.Where(t => t.TestAttemptId == item.Id).FirstOrDefaultAsync(), Test = item.Test });
                }
                else
                    listOfTest.Add(new UserTest { isAttempted = true, Test = item.Test });
            }
            foreach (var item in tests)
            {
                if (!listOfTest.Exists(c => c.Test.Id == item.Id))
                {
                    listOfTest.Add(new UserTest { isAttempted = false, Test = item });
                }
            }
            var vm = new SelectChapterViewModel
            {
                Tests = listOfTest,
                ClassName = classa.Name
            };
            return View(vm);
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StartTest(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var test = await _context.Tests.Where(c => c.Id == id).Include(t => t.Questions).Include(t => t.Chapter).AsNoTracking().FirstOrDefaultAsync();
            if (test == null)
                return NotFound();
            var user = await _userManager.GetUserAsync(User);
            var userTests = await _context.Users.Where(u => u.Id == user.Id).SelectMany(u => u.TestAttempts).Include(t => t.Test).ToListAsync();
            if (userTests.Exists(t => t.isCompleted == false))
            {
                return RedirectToAction(nameof(OnGoingTest), new { id = userTests.FirstOrDefault(c => c.isCompleted == false).Id });
            }
            if (userTests.Exists(t => t.Test.Id == test.Id))
            {
                return RedirectToAction(nameof(SelectChapter), new { id = test.Chapter.ClassId });
            }
            var testAttempt = new TestAttempt { TestId = test.Id, Student = await _userManager.GetUserAsync(User), StartTime = DateTime.Now };
            await _context.TestAttempts.AddAsync(testAttempt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OnGoingTest), new { id = testAttempt.Id });
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> OnGoingTest(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var testAttempt = await _context.TestAttempts.Where(t => t.Id == id && t.isCompleted == false).Include(t => t.Test).ThenInclude(t => t.Questions).Include(t => t.Test.Chapter).ThenInclude(c => c.Class).FirstOrDefaultAsync();
            if (testAttempt == null)
                return NotFound();
            var remainingTime = testAttempt.StartTime.Add(testAttempt.Test.TestDuration).Subtract(DateTime.Now);
            if (remainingTime.Minutes > 0)
            {
                return View(new StartTestViewModel { Test = testAttempt.Test, AttemptId = testAttempt.Id, Hours = remainingTime.Hours, Minutes = remainingTime.Minutes, Seconds = remainingTime.Seconds });
            }
            testAttempt.isCompleted = true;
            _context.TestAttempts.Update(testAttempt);
            var testResult = new TestResult { TestAttemptId = testAttempt.Id };
            testResult.TimeTaken = testAttempt.Test.TestDuration;
            testResult.IncorrectAnswers = testAttempt.Test.Questions.Count();
            testResult.CorrectAnswers = 0;
            await _context.TestResults.AddAsync(testResult);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SelectChapter), new { id = testAttempt.Test.Chapter.ClassId });
        }

        [Authorize(Roles = "Student,Admin")]
        [HttpPost]
        public async Task<JsonResult> SubmitTest([FromBody] SubmitTestViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var testAttempt = await _context.TestAttempts.Where(ta => ta.Id == vm.TestAttemptId).FirstAsync();
                if (testAttempt == null || testAttempt.isCompleted)
                {
                    return Json(new { isError = true });
                }
                var test = await _context.Tests.Where(c => c.Id == vm.TestId).Include(t => t.Questions).Include(c => c.Chapter).ThenInclude(c => c.Class).AsNoTracking().AsNoTracking().FirstOrDefaultAsync();
                if (test == null)
                    return Json(new { isError = true });

                var testResult = new TestResult { TestAttemptId = testAttempt.Id };
                if (DateTime.Now < testAttempt.StartTime.Add(test.TestDuration))
                {
                    var questions = test.Questions;
                    var i = 0;
                    foreach (var item in test.Questions)
                    {
                        if (item.Answer == vm.Questions.ElementAt(i).Answer)
                        {
                            testResult.CorrectAnswers++;
                        }
                        else
                        {
                            testResult.IncorrectAnswers++;
                        }
                        i++;
                    }
                }
                else
                {
                    testResult.IncorrectAnswers = test.Questions.Count();
                }
                testResult.TimeTaken = DateTime.Now.Subtract(testAttempt.StartTime).Duration();
                if (testResult.TimeTaken > test.TestDuration)
                    testResult.TimeTaken = test.TestDuration;
                testAttempt.isCompleted = true;
                _context.TestAttempts.Update(testAttempt);
                await _context.TestResults.AddAsync(testResult);
                await _context.SaveChangesAsync();
                return Json(new { isError = false, location = Url.Action(nameof(Index)) });
            }
            return Json(new { isError = true });
        }


        // GET: Tests/Insights/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Insights(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .Include(t => t.Chapter)
                .ThenInclude(c => c.Class)
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (test == null)
            {
                return NotFound();
            }
            var vm = new TestInsightsViewModel
            {
                Test = test,
                Results = await _context.TestResults.Where(t => t.TestAttempt.Test.Id == id).Include(t => t.TestAttempt).ThenInclude(t => t.Student).Include(t => t.TestAttempt.Test).OrderByDescending(t => t.CorrectAnswers).ToListAsync(),
                TotalQuestions = await _context.TestQuestions.Where(t => t.TestId == test.Id).CountAsync()
            };
            return View(vm);
        }

        // GET: Tests/Create
        [Authorize(Roles = "Admin")]
        [Route("Tests/Create/Admin", Name = "CreateTest")]
        [HttpGet]
        public async Task<IActionResult> CreateTest()
        {
            return View(await populateSBS(new TestQuestionViewModel()));
        }

        // POST: Tests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Tests/Create/Admin", Name = "CreateTest")]
        public async Task<JsonResult> CreateTest([FromBody] TestViewModel test)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var chapterExists = await _context.Chapters.AnyAsync(c => c.Id == test.ChapterId);
                if (!chapterExists)
                {
                    return Json(new { IsError = true });
                }
                var Questions = _mapper.Map<ICollection<TestQuestionViewModel>, List<TestQuestion>>(test.Questions);
                var userId = await _context.Users.Where(u => u.Email == User.Identity.Name).Select(u => u.Id).SingleAsync();
                var time = new TimeSpan(test.Hours, test.Minutes, 0);
                await _context.AddAsync(new Test { Questions = Questions, ChapterId = test.ChapterId, CreatedById = userId, TestDuration = time });
                await _context.SaveChangesAsync();
                return Json(new { IsError = false });
            }
            return Json(new { IsError = true, ErrorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(s => s.ErrorMessage)) });
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

        [Authorize(Roles = "Faculty")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var classa = await _context.Classes.Where(c => c.StandardId == int.Parse(User.FindFirst("StandardId").Value) && c.BoardId == int.Parse(User.FindFirst("BoardId").Value) && c.SubjectId == int.Parse(User.FindFirst("SubjectId").Value)).AsNoTracking().Select(c => c.Id).SingleAsync();
            return View(new TestQuestionViewModel { Chapters = new SelectList(await _context.Chapters.Where(c => c.ClassId == classa).ToListAsync(), "Id", "Name") });
        }



        [HttpPost]
        [Consumes("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] TestViewModel test)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var chapterExists = await _context.Chapters.AnyAsync(c => c.Id == test.ChapterId);
                if (!chapterExists)
                {
                    return Json(new { IsError = true });
                }
                var Questions = _mapper.Map<ICollection<TestQuestionViewModel>, List<TestQuestion>>(test.Questions);
                var userId = await _context.Users.Where(u => u.Email == User.Identity.Name).Select(u => u.Id).SingleAsync();
                var time = new TimeSpan(test.Hours, test.Minutes, 00);
                await _context.AddAsync(new Test { Questions = Questions, ChapterId = test.ChapterId, CreatedById = userId, TestDuration = time });
                await _context.SaveChangesAsync();
                return Json(new { IsError = false });
            }
            return Json(new { IsError = true, ErrorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(s => s.ErrorMessage)) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetTestAttempt(int id)
        {
            if (id == null)
                return Json(new { isError = true });
            var testAttempt = await _context.TestAttempts.FindAsync(id);
            try
            {
                _context.TestAttempts.Remove(testAttempt);
                await _context.SaveChangesAsync();
                return Json(new { isError = false });
            }
            catch
            {
                return Json(new { isError = true });
            }
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

        public async Task<TestQuestionViewModel> populateSBS(TestQuestionViewModel model, int? standardId = null, int? boardId = null, int? subjectId = null)
        {
            model.Standards = await getStandards(standardId);
            model.Subjects = await getSubjects(subjectId);
            model.Boards = await getBoards(boardId);
            return model;
        }


        // POST: Tests/Delete/5
        [Authorize(Roles = "Admin,Faculty")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var test = await _context.Tests.FindAsync(id);
            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TestExists(long id)
        {
            return await _context.Tests.AnyAsync(e => e.Id == id);
        }
    }
}
