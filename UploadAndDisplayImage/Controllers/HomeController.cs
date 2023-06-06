using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using UploadAndDisplayImage.Models;
using UploadAndDisplayImage.ViewModel;

namespace UploadAndDisplayImage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StudentDbContext Context;
        private readonly IWebHostEnvironment WebHostEnvironment;

        public HomeController(ILogger<HomeController> logger, StudentDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            Context = context;
            WebHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var items = Context.Students.ToList();
            return View(items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentViewModel vm)
        {
            string stringFileName = UploadFile(vm);
            var student = new Student
            {
                Name = vm.Name,
                ProfileImage = stringFileName
            };
            Context.Students.Add(student);
            Context.SaveChanges();

            return RedirectToAction("Index");
        }

        private string UploadFile(StudentViewModel vm)
        {
            string fileName = null;
            if (vm.ProfileImage != null)
            {
                string uploadDir = Path.Combine(WebHostEnvironment.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "-" + vm.ProfileImage.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    vm.ProfileImage.CopyTo(fileStream);
                }
            }
            return fileName;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}