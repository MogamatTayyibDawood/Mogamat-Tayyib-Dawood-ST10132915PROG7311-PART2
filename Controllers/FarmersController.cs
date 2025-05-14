using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG7311_PART2_AgriEnergyConnect.Data;
using PROG7311_PART2_AgriEnergyConnect.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PROG7311_PART2_AgriEnergyConnect.Controllers
{
    [Authorize(Roles = "Employee")]
    public class FarmersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FarmersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Farmers
        public async Task<IActionResult> Index(string searchTerm, string sortOrder, int? pageNumber)
        {
            ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchTerm;

            var farmers = from f in _context.Farmers
                          select f;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                farmers = farmers.Where(f =>
                    f.Name.Contains(searchTerm) ||
                    f.Email.Contains(searchTerm) ||
                    f.ContactNumber.Contains(searchTerm));
            }

            farmers = sortOrder switch
            {
                "name_desc" => farmers.OrderByDescending(f => f.Name),
                _ => farmers.OrderBy(f => f.Name),
            };

            int pageSize = 10;
            return View(await PaginatedList<Farmer>.CreateAsync(farmers.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Farmers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers
                .Include(f => f.Products)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (farmer == null)
            {
                return NotFound();
            }

            return View(farmer);
        }

        // GET: Farmers/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,ContactNumber")] Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(farmer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Farmer created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(farmer);
        }

        // GET: Farmers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
            {
                return NotFound();
            }
            return View(farmer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,ContactNumber")] Farmer farmer)
        {
            if (id != farmer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(farmer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Farmer updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmerExists(farmer.Id))
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
            return View(farmer);
        }

        // GET: Farmers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (farmer == null)
            {
                return NotFound();
            }

            return View(farmer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer != null)
            {
                try
                {
                    _context.Farmers.Remove(farmer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Farmer deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Cannot delete this farmer because they have associated products.";
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FarmerExists(int id)
        {
            return _context.Farmers.Any(e => e.Id == id);
        }
    }
}