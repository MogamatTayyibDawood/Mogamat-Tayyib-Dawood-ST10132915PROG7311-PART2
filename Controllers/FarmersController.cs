using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG7311_PART2_AgriEnergyConnect.Data;
using PROG7311_PART2_AgriEnergyConnect.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PROG7311_PART2_AgriEnergyConnect.Controllers
{
    [Authorize(Roles = "Employee")]
    public class FarmersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FarmersController> _logger;

        public FarmersController(ApplicationDbContext context, ILogger<FarmersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Farmers
        public async Task<IActionResult> Index(string searchString, int? pageNumber, string sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.EmailSortParam = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.ContactSortParam = sortOrder == "ContactNumber" ? "contact_desc" : "ContactNumber";

            var farmers = _context.Farmers.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                farmers = farmers.Where(f =>
                    f.Name.Contains(searchString) ||
                    f.Email.Contains(searchString) ||
                    f.ContactNumber.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    farmers = farmers.OrderByDescending(f => f.Name);
                    break;
                case "Email":
                    farmers = farmers.OrderBy(f => f.Email);
                    break;
                case "email_desc":
                    farmers = farmers.OrderByDescending(f => f.Email);
                    break;
                case "ContactNumber":
                    farmers = farmers.OrderBy(f => f.ContactNumber);
                    break;
                case "contact_desc":
                    farmers = farmers.OrderByDescending(f => f.ContactNumber);
                    break;
                default:
                    farmers = farmers.OrderBy(f => f.Name);
                    break;
            }

            int pageSize = 10;
            ViewBag.TotalFarmers = farmers.Count();
            var paginatedList = await PaginatedList<Farmer>.CreateAsync(farmers.AsNoTracking(), pageNumber ?? 1, pageSize);
            ViewBag.CurrentPage = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;

            return View(paginatedList);
        }

        // GET: Farmers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogError("Details requested for null farmer ID.");
                return NotFound();
            }

            var farmer = await _context.Farmers
                .Include(f => f.Products)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (farmer == null)
            {
                _logger.LogError($"Farmer with ID {id} not found.");
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
            // Check for existing email
            if (await _context.Farmers.AnyAsync(f => f.Email == farmer.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(farmer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Farmer created successfully!";
                _logger.LogInformation($"Farmer created: {farmer.Name} ({farmer.Email})");
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Farmer creation failed due to validation errors.");
            return View(farmer);
        }

        // GET: Farmers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogError("Edit requested for null farmer ID.");
                return NotFound();
            }

            var farmer = await _context.Farmers.FindAsync(id);

            if (farmer == null)
            {
                _logger.LogError($"Farmer with ID {id} not found for edit.");
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
                _logger.LogError($"Edit attempted with mismatched farmer IDs: {id} vs. {farmer.Id}");
                return NotFound();
            }

            // Check for duplicate email
            if (await _context.Farmers.AnyAsync(f => f.Email == farmer.Email && f.Id != farmer.Id))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(farmer);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(farmer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Farmer updated successfully!";
                    _logger.LogInformation($"Farmer updated: {farmer.Name} ({farmer.Email})");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmerExists(farmer.Id))
                    {
                        _logger.LogError($"Farmer with ID {farmer.Id} not found during edit.");
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving changes.";
                        _logger.LogError($"Concurrency error while updating farmer: {farmer.Name} ({farmer.Email})");
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning($"Farmer edit failed due to validation errors: {farmer.Name} ({farmer.Email})");
            return View(farmer);
        }

        // GET: Farmers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogError("Delete requested for null farmer ID.");
                return NotFound();
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (farmer == null)
            {
                _logger.LogError($"Farmer with ID {id} not found for deletion.");
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
                    _logger.LogInformation($"Farmer deleted: {farmer.Name} ({farmer.Email})");
                }
                catch (DbUpdateException ex)
                {
                    TempData["ErrorMessage"] = "Cannot delete this farmer because they have associated products.";
                    _logger.LogError(ex, $"Attempt to delete farmer with associated products: {farmer.Name} ({farmer.Email})");
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                _logger.LogError($"Attempt to delete non-existent farmer with ID: {id}");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FarmerExists(int id)
        {
            return _context.Farmers.Any(e => e.Id == id);
        }
    }
}