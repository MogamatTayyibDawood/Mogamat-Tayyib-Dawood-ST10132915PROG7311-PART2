using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG7311_PART2_AgriEnergyConnect.Data;
using PROG7311_PART2_AgriEnergyConnect.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PROG7311_PART2_AgriEnergyConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Employee"))
                {
                    ViewBag.FarmerCount = await _context.Farmers.CountAsync();
                    ViewBag.ProductCount = await _context.Products.CountAsync();

                    var categoryCounts = await _context.Products
                        .GroupBy(p => p.Category)
                        .Select(g => new { Category = g.Key, Count = g.Count() })
                        .ToListAsync();

                    ViewBag.Categories = categoryCounts.Select(c => c.Category).ToList();
                    ViewBag.CategoryCounts = categoryCounts.Select(c => c.Count).ToList();
                }
                else if (User.IsInRole("Farmer"))
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                    if (farmer != null)
                    {
                        ViewBag.ProductCount = await _context.Products.CountAsync(p => p.FarmerId == farmer.Id);

                        var categoryCounts = await _context.Products
                            .Where(p => p.FarmerId == farmer.Id)
                            .GroupBy(p => p.Category)
                            .Select(g => new { Category = g.Key, Count = g.Count() })
                            .ToListAsync();

                        ViewBag.Categories = categoryCounts.Select(c => c.Category).ToList();
                        ViewBag.CategoryCounts = categoryCounts.Select(c => c.Count).ToList();
                    }
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            _logger.LogError(exception, "An error occurred processing request: {Path}",
                exceptionHandlerPathFeature?.Path);

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = exception?.Message
            });
        }
    }
}