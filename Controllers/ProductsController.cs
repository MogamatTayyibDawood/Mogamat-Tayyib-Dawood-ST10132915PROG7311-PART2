using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROG7311_PART2_AgriEnergyConnect.Data;
using PROG7311_PART2_AgriEnergyConnect.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PROG7311_PART2_AgriEnergyConnect.Controllers
{
    [Authorize(Roles = "Farmer,Employee")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<ProductsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Products
        public async Task<IActionResult> Index(
            string categoryFilter,
            DateTime? startDate,
            DateTime? endDate,
            string searchString,
            string sortOrder,
            int? pageNumber)
        {
            ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSort"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["CategorySort"] = sortOrder == "category" ? "category_desc" : "category";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoryFilter"] = categoryFilter;

            var products = _context.Products
                .Include(p => p.Farmer)
                .AsQueryable();

            // Role-based filtering
            if (User.IsInRole("Farmer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);
                if (farmer != null)
                    products = products.Where(p => p.FarmerId == farmer.Id);
            }

            // Search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Category.Contains(searchString) ||
                    p.Farmer.Name.Contains(searchString));
            }

            // Category filter
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                products = products.Where(p => p.Category == categoryFilter);
            }

            // Date range filter
            if (startDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate <= endDate.Value);
            }

            // Sorting
            products = sortOrder switch
            {
                "name_desc" => products.OrderByDescending(p => p.Name),
                "date" => products.OrderBy(p => p.ProductionDate),
                "date_desc" => products.OrderByDescending(p => p.ProductionDate),
                "category" => products.OrderBy(p => p.Category),
                "category_desc" => products.OrderByDescending(p => p.Category),
                _ => products.OrderBy(p => p.Name),
            };

            // For category dropdown
            ViewBag.Categories = new SelectList(await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync());

            // Pass success message from TempData to the ViewData
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];

            int pageSize = 10;
            return View(await PaginatedList<Product>.CreateAsync(products.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Farmer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);
                if (farmer != null)
                {
                    ViewData["FarmerId"] = new SelectList(new[] { farmer }, "Id", "Name");
                }
            }
            else
            {
                ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name");
            }

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
    
        public async Task<IActionResult> Create([Bind("Name,Category,ProductionDate,FarmerId")] Product product)

        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Farmer"))
                {
                    var user = await _userManager.GetUserAsync(User);
                    var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);

                    if (farmer == null)
                    {
                        ModelState.AddModelError("", "Farmer profile not found.");
                        return View(product);
                    }

                    product.FarmerId = farmer.Id; // Automatically set the FarmerId for Farmer role
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product created successfully!"; // Add success message
                return RedirectToAction("Index"); // Ensure it's redirecting to Index view
            }

            // Repopulate dropdown if validation fails
            return View(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
