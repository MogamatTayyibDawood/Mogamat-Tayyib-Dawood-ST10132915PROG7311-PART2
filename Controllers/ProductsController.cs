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

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index(string categoryFilter, DateTime? startDate, DateTime? endDate, string searchString, int? pageNumber)
        {
            var products = _context.Products.Include(p => p.Farmer).AsQueryable();

            if (User.IsInRole("Farmer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                if (farmer != null)
                {
                    products = products.Where(p => p.FarmerId == farmer.Id);
                }
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                products = products.Where(p => p.Category == categoryFilter);
            }

            if (startDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Category.Contains(searchString) ||
                    p.Farmer.Name.Contains(searchString));
            }

            int pageSize = 10;
            return View(await PaginatedList<Product>.CreateAsync(products.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
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

            if (User.IsInRole("Farmer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                if (farmer == null || product.FarmerId != farmer.Id)
                {
                    return Forbid();
                }
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Farmer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                if (farmer == null)
                {
                    TempData["ErrorMessage"] = "You don't have a farmer profile. Please contact an administrator.";
                    return RedirectToAction(nameof(Index));
                }

                ViewData["FarmerId"] = new SelectList(new List<Farmer> { farmer }, "Id", "Name");
            }
            else
            {
                ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Category,ProductionDate,FarmerId")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Farmer"))
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                    if (farmer == null || product.FarmerId != farmer.Id)
                    {
                        ModelState.AddModelError("", "You can only add products for your own farm.");
                        ViewData["FarmerId"] = new SelectList(new List<Farmer> { farmer }, "Id", "Name");
                        return View(product);
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }

            if (User.IsInRole("Farmer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                if (farmer != null)
                {
                    ViewData["FarmerId"] = new SelectList(new List<Farmer> { farmer }, "Id", "Name", product.FarmerId);
                }
            }
            else
            {
                ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name", product.FarmerId);
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

            if (User.IsInRole("Farmer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                if (farmer == null || product.FarmerId != farmer.Id)
                {
                    return Forbid();
                }

                ViewData["FarmerId"] = new SelectList(new List<Farmer> { farmer }, "Id", "Name", product.FarmerId);
            }
            else
            {
                ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name", product.FarmerId);
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,ProductionDate,FarmerId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (User.IsInRole("Farmer"))
                    {
                        var currentUser = await _userManager.GetUserAsync(User);
                        var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                        if (farmer == null || product.FarmerId != farmer.Id)
                        {
                            ModelState.AddModelError("", "You can only edit products for your own farm.");
                            ViewData["FarmerId"] = new SelectList(new List<Farmer> { farmer }, "Id", "Name", product.FarmerId);
                            return View(product);
                        }
                    }

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

            if (User.IsInRole("Employee"))
            {
                ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name", product.FarmerId);
            }
            else
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                if (farmer != null)
                {
                    ViewData["FarmerId"] = new SelectList(new List<Farmer> { farmer }, "Id", "Name", product.FarmerId);
                }
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

            if (User.IsInRole("Farmer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                if (farmer == null || product.FarmerId != farmer.Id)
                {
                    return Forbid();
                }
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                if (User.IsInRole("Farmer"))
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);

                    if (farmer == null || product.FarmerId != farmer.Id)
                    {
                        return Forbid();
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Filter()
        {
            ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name");
            ViewData["Categories"] = new SelectList(await _context.Products.Select(p => p.Category).Distinct().ToListAsync());
            return View(new List<Product>());
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Filter(int? farmerId, string category, DateTime? startDate, DateTime? endDate)
        {
            var products = _context.Products.Include(p => p.Farmer).AsQueryable();

            if (farmerId.HasValue)
            {
                products = products.Where(p => p.FarmerId == farmerId.Value);
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            if (startDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate <= endDate.Value);
            }

            ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name", farmerId);
            ViewData["Categories"] = new SelectList(await _context.Products.Select(p => p.Category).Distinct().ToListAsync(), category);
            return View(await products.ToListAsync());
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}