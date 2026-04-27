using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildForgeApp.Data;
using BuildForgeApp.Models;

namespace BuildForgeApp.Controllers
{
    public class PcComponentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PcComponentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? buildId, string? category, string? searchString, string? brand)
        {
            var query = _context.PcComponents
                .Where(pc => pc.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(pc => pc.ComponentType == category);

            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(pc =>
                    pc.Name.Contains(searchString) ||
                    pc.Brand.Contains(searchString) ||
                    pc.ComponentType.Contains(searchString));

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(pc => pc.Brand == brand);

            var components = await query
                .OrderBy(pc => pc.ComponentType)
                .ThenBy(pc => pc.Brand)
                .ThenBy(pc => pc.Name)
                .ToListAsync();

            ViewBag.BuildId = buildId;

            ViewBag.Categories = await _context.PcComponents
                .Where(pc => pc.IsActive)
                .Select(pc => pc.ComponentType)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            ViewBag.Brands = await _context.PcComponents
                .Where(pc => pc.IsActive)
                .Select(pc => pc.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();

            return View(components);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var component = await _context.PcComponents
                .FirstOrDefaultAsync(pc => pc.Id == id && pc.IsActive);

            if (component == null) return NotFound();

            return View(component);
        }

        [HttpPost]
        public async Task<IActionResult> AddToBuild(int buildId, int componentId)
        {
            var build = await _context.Builds
                .Include(b => b.BuildComponents)
                .FirstOrDefaultAsync(b => b.Id == buildId);

            var component = await _context.PcComponents
                .FirstOrDefaultAsync(c => c.Id == componentId);

            if (build == null || component == null)
                return NotFound();

            var existing = await _context.BuildComponents
                .FirstOrDefaultAsync(bc => bc.BuildId == buildId && bc.PcComponentId == componentId);

            if (existing == null)
            {
                _context.BuildComponents.Add(new BuildComponent
                {
                    BuildId = buildId,
                    PcComponentId = componentId
                });

                build.TotalPrice += component.Price;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Builds", new { id = buildId });
        }
    }
}
