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

        public ProductsController(ApplicationDbContext context,
                                UserManager<ApplicationUser> userManager,
                                ILogger<ProductsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Products
        // THIS IS WHERE THE INDEX METHOD GOES:
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
                    ViewBag.FarmerName = farmer.Name;
                }
            }
            else if (User.IsInRole("Employee"))
            {
                ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name");
            }

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Category,ProductionDate,FarmerId")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // For Farmers, automatically set their ID
                    if (User.IsInRole("Farmer"))
                    {
                        var user = await _userManager.GetUserAsync(User);
                        var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);

                        if (farmer == null)
                        {
                            ModelState.AddModelError("", "Farmer profile not found.");
                            _logger.LogWarning($"Farmer not found for user {user.Email}");
                            return View(product);
                        }

                        product.FarmerId = farmer.Id;
                        _logger.LogInformation($"Assigned FarmerId {farmer.Id} to product");
                    }
                    // For Employees, ensure they selected a farmer
                    else if (User.IsInRole("Employee") && product.FarmerId == 0)
                    {
                        ModelState.AddModelError("FarmerId", "Please select a farmer.");
                        ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name");
                        return View(product);
                    }

                

                    _context.Add(product);
                    var result = await _context.SaveChangesAsync();
                    _logger.LogInformation($"SaveChangesAsync result: {result} rows affected");

                    if (result > 0)
                    {
                        TempData["SuccessMessage"] = "Product created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogWarning("SaveChangesAsync affected 0 rows");
                        ModelState.AddModelError("", "Failed to save product to database.");
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating product");
                ModelState.AddModelError("", "A database error occurred while creating the product.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating product");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }

            // Repopulate dropdowns if we return to the view
            if (User.IsInRole("Farmer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);
                if (farmer != null)
                {
                    ViewData["FarmerId"] = new SelectList(new[] { farmer }, "Id", "Name");
                    ViewBag.FarmerName = farmer.Name;
                }
            }
            else
            {
                ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Check if user is authorized to edit this product
            if (User.IsInRole("Farmer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);
                if (farmer == null || product.FarmerId != farmer.Id)
                {
                    return Forbid();
                }
                ViewData["FarmerId"] = new SelectList(new[] { farmer }, "Id", "Name");
                ViewBag.FarmerName = farmer.Name;
            }
            else
            {
                ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name");
            }

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,ProductionDate,FarmerId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            // Check if user is authorized to edit this product
            if (User.IsInRole("Farmer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);
                if (farmer == null || product.FarmerId != farmer.Id)
                {
                    return Forbid();
                }
                product.FarmerId = farmer.Id; // Ensure FarmerId can't be changed
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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

            // Repopulate dropdown if validation fails
            if (User.IsInRole("Farmer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);
                if (farmer != null)
                {
                    ViewData["FarmerId"] = new SelectList(new[] { farmer }, "Id", "Name");
                    ViewBag.FarmerName = farmer.Name;
                }
            }
            else
            {
                ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name");
            }

            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Check if user is authorized to delete this product
            if (User.IsInRole("Farmer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);
                if (farmer == null || product.FarmerId != farmer.Id)
                {
                    return Forbid();
                }
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Check if user is authorized to delete this product
            if (User.IsInRole("Farmer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);
                if (farmer == null || product.FarmerId != farmer.Id)
                {
                    return Forbid();
                }
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                TempData["ErrorMessage"] = "An error occurred while deleting the product.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}