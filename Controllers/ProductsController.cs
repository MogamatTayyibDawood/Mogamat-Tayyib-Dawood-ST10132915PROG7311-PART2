using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROG7311_PART2_AgriEnergyConnect.Data;
using PROG7311_PART2_AgriEnergyConnect.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace PROG7311_PART2_AgriEnergyConnect.Controllers
{
    [Authorize]
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
        // GET: Products
        public async Task<IActionResult> Index()
        {
            var query = _context.Products.Include(p => p.Farmer).AsQueryable();

            if (User.IsInRole("Farmer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);
                if (farmer != null)
                {
                    query = query.Where(p => p.FarmerId == farmer.Id);
                }
            }

            return View(await query.ToListAsync());
        }


        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Employee"))
            {
                var farmers = await _context.Farmers.ToListAsync();
                ViewData["FarmerId"] = new SelectList(farmers, "Id", "Name");
            }
            else if (User.IsInRole("Farmer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);
                ViewData["FarmerId"] = new SelectList(new[] { farmer }, "Id", "Name", farmer?.Id);
            }

            return View();
        }


        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Category,ProductionDate,FarmerId")] Product product)
        {
            if (User.IsInRole("Farmer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);
                if (farmer == null)
                {
                    ModelState.AddModelError("", "Your farmer profile could not be found.");
                    return View(product);
                }

                product.FarmerId = farmer.Id;
            }

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Reload FarmerId dropdown if model state fails
            if (User.IsInRole("Employee"))
            {
                ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name", product.FarmerId);
            }
            else if (User.IsInRole("Farmer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == currentUser.Email);
                ViewData["FarmerId"] = new SelectList(new[] { farmer }, "Id", "Name", farmer.Id);
            }

            return View(product);
        }

      

// GET: Products/Details/5
public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name", product.FarmerId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,ProductionDate,FarmerId")] Product product)
        {
            if (id != product.Id) return NotFound();

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
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["FarmerId"] = new SelectList(_context.Farmers, "Id", "Name", product.FarmerId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
