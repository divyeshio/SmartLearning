using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.LiveClassAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Controllers
{
    [Authorize]
    public class LiveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IWebHostEnvironment _env;

        public LiveController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _env = webHostEnvironment;
        }


        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (user.AccountType == AccountTypeEnum.Admin)
                {
                    return View(await _context.LiveClasses.Include(l => l.Class).ThenInclude(c => c.Subject).ToListAsync());
                }
                else
                {
                    var vm = await _context.LiveClasses.Where(live => live.Class.StandardId == user.StandardId && live.Class.BoardId == user.BoardId).Include(l => l.Class).ThenInclude(c => c.Subject).Include(l => l.Class.Standard).Include(l => l.Class.Board).ToListAsync();
                    return View(vm);
                }
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List()
        {
            return View(await _context.LiveClasses.Include(c => c.Broadcaster).AsNoTracking().ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [Route("/GoLive/Admin", Name = "GoLiveAdmin")]
        [HttpGet]
        public async Task<IActionResult> GoLiveAdmin()
        {
            var userId = _userManager.GetUserId(User);
            var liveClass = await _context.LiveClasses.Where(l => l.BroadcasterId == userId).FirstOrDefaultAsync();
            if (liveClass != null)
            {
                return RedirectToAction(nameof(Class), new { id = liveClass.Id });
            }
            ViewData["Boards"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name");
            ViewData["Standards"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Name");
            ViewData["Subjects"] = new SelectList(_context.Subjects.OrderBy(b => b.Name), "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> GoLive()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Select(u => new ApplicationUser { Id = u.Id, BoardId = u.BoardId, StandardId = u.StandardId, SubjectId = u.SubjectId, AccountType = u.AccountType }).Where(u => u.Id == userId).FirstOrDefaultAsync();
            var liveClass = await _context.LiveClasses.Where(l => l.BroadcasterId == userId).FirstOrDefaultAsync();
            if (liveClass != null)
            {
                return RedirectToAction(nameof(Class), new { id = liveClass.Id });
            }

            if (user != null)
            {
                if (user.AccountType == AccountTypeEnum.Admin)
                {
                    return View("GoLiveAdmin", new LiveClassViewModel());
                }
                else
                {
                    var classa = await _context.Classes.Where(c => c.StandardId == user.StandardId && c.BoardId == user.BoardId && c.SubjectId == user.SubjectId).Include(c => c.Standard).Include(c => c.Board).Include(c => c.Subject).FirstOrDefaultAsync();
                    if (classa != null)
                        return View("GoLiveFaculty", new LiveClassViewModel { ClassName = classa.Name, ClassId = classa.Id });
                }
            }
            return NotFound();
        }


        [Authorize(Roles = "Admin,Faculty")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> GoLive([Bind("ClassId")] LiveClassViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            var classa = await _context.Classes.Where(c => c.BoardId == user.BoardId && c.SubjectId == user.SubjectId && c.StandardId == user.StandardId).FirstOrDefaultAsync();
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    var liveClass = new LiveClass { Broadcaster = user, isAdmin = false, Class = classa };
                    await _context.LiveClasses.AddAsync(liveClass);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Class), new { id = liveClass.Id });
                }
            }
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [Route("/GoLive/Admin", Name = "GoLiveAdmin")]
        [HttpPost]
        public async Task<IActionResult> GoLiveAdminPost(AdminLiveClassViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    var liveClass = new LiveClass { Broadcaster = user, isAdmin = true };
                    await _context.LiveClasses.AddAsync(liveClass);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Class), new { id = liveClass.Id });
                }
            }
            ViewData["Boards"] = new SelectList(_context.Boards.OrderBy(b => b.AbbrName), "Id", "Name", vm.Boards);
            ViewData["Standards"] = new SelectList(_context.Standards.OrderBy(b => b.Level), "Id", "Name", vm.Standard);
            ViewData["Subjects"] = new SelectList(_context.Subjects.OrderBy(b => b.Name), "Id", "Name", vm.Subject);
            return View(vm);
        }

        [Authorize(Roles = "Admin,Faculty,Student")]
        public async Task<IActionResult> Class(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Select(u => new ApplicationUser { Id = u.Id, Board = u.Board, Standard = u.Standard, Subject = u.Subject, AccountType = u.AccountType }).Where(u => u.Id == userId).AsNoTracking().FirstOrDefaultAsync();
            var liveClass = await _context.LiveClasses.Where(l => l.Id == id).Include(l => l.Class).ThenInclude(c => c.Subject).AsNoTracking().FirstOrDefaultAsync();
            if (liveClass != null)
            {
                switch (user.AccountType)
                {
                    case AccountTypeEnum.Faculty: return View("FacultyClass", liveClass);

                    case AccountTypeEnum.Student: return View("StudentClass", liveClass);
                        //case AccountTypeEnum.Admin: return View("AdminClass", liveClass);
                }
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> FaceCheck(string id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));
            var user = await _userManager.GetUserAsync(User);
            if (user.FaceDataId == null)
                return RedirectToAction("AddFaceData");
            return View("FaceCheck", id);
        }

        [HttpPost]
        public async Task<JsonResult> FaceCheck(IFormFile FaceImage, int ClassId)
        {
            if (FaceImage == null || ClassId == null)
                return Json(new { isError = true });
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "Storage", "FaceData", "Temp");
            var fileName = Guid.NewGuid().ToString() + FaceImage.FileName + ".png";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                FaceImage.CopyTo(fileStream);
            }
            var user = await _userManager.GetUserAsync(User);
            var liveClass = await _context.LiveClasses.AsNoTracking().Where(l => l.ClassId == ClassId).Select(s => s.Id).FirstOrDefaultAsync();
            if (liveClass == null)
                return Json(new { isError = true });
            var isAllowed = await CompareFace(fileName, user.Id);
            if (isAllowed)
            {
                return Json(new { isError = false, location = Url.ActionLink("Class", "Live", new { id = liveClass }) });
            }
            else return Json(new { isError = true });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> StopLive(int id, string broadcasterId)
        {
            if (broadcasterId == null || id == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Select(u => new { u.Id, u.AccountType }).Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (userId == broadcasterId || user.AccountType == AccountTypeEnum.Admin)
            {
                var live = await _context.LiveClasses.Where(lc => lc.BroadcasterId == broadcasterId && lc.Id == id).FirstOrDefaultAsync();
                if (live != null)
                {
                    _context.LiveClasses.Remove(live);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Dashboard");
                }
                else return Content("No such live class");
            }
            else return Content("You are not allowed");
        }

        public IActionResult AddFaceData()
        {
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> AddFaceData([FromForm] IFormFile FaceImage)
        {
            /*if (FaceImage != null)
            {
              string uploadsFolder = Path.Combine(_env.ContentRootPath, "Storage", "FaceData", "Temp");
              string fileName = Guid.NewGuid().ToString() + FaceImage.FileName + ".png";
              string filePath = Path.Combine(uploadsFolder, fileName);
              using (var fileStream = new FileStream(filePath, FileMode.Create))
              {
                FaceImage.CopyTo(fileStream);
              }
              Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
              FaceRecognition.InternalEncoding = Encoding.GetEncoding(932);
              string directory = "C:\\Users\\Divyesh\\source\\repos\\SmartLearning\\models";
              FaceRecognition fr = FaceRecognition.Create(directory);
              Image imageA = FaceRecognition.LoadImageFile(filePath);
              IEnumerable<Location> locationsA = fr.FaceLocations(imageA);
              if (locationsA.Count() != 1)
                return Json(new { isError = true });
              IEnumerable<FaceEncoding> encodingsA = fr.FaceEncodings(imageA, locationsA);
              var user = await _userManager.GetUserAsync(User);
              var FaceData = new FaceData
              {
                FaceEncoding = encodingsA.First().GetRawEncoding(),
                User = user
              };
              await _context.Faces.AddAsync(FaceData);
              await _context.SaveChangesAsync();
              var face = await _context.Faces.Where(f => f.UserId == user.Id).FirstOrDefaultAsync();
              user.FaceDataId = face.Id;
              await _userManager.UpdateAsync(user);
              await _context.SaveChangesAsync();
              if (System.IO.File.Exists(filePath))
              {
                System.IO.File.Delete(filePath);
              }
              return Json(new { isError = false });
            }*/
            return Json(new { isError = true });
        }


        public async Task<bool> CompareFace(string actualFace, string id)
        {
            /*Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FaceRecognition.InternalEncoding = Encoding.GetEncoding(932);
            string directory = "C:\\Users\\Divyesh\\source\\repos\\SmartLearning\\models";
            string imagePathB = "C:\\Users\\Divyesh\\source\\repos\\SmartLearning\\SmartLearning\\Storage\\FaceData\\Temp\\" + actualFace;
            FaceRecognition fr = FaceRecognition.Create(directory);
            Image imageA = FaceRecognition.LoadImageFile(imagePathB);
            IEnumerable<Location> locationsA = fr.FaceLocations(imageA);
            IEnumerable<FaceEncoding> encodingA = fr.FaceEncodings(imageA, locationsA);
            var encoding = await _context.Faces.Where(f => f.UserId == id).AsNoTracking().FirstOrDefaultAsync();
            FaceEncoding encodingB = FaceRecognition.LoadFaceEncoding(encoding.FaceEncoding);
            const double tolerance = 0.6d;
            bool match = FaceRecognition.CompareFace(encodingA.First(), encodingB, tolerance);
            if (System.IO.File.Exists(actualFace))
            {
              System.IO.File.Delete(actualFace);
            }*/
            return true;
        }

    }
}
