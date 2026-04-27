using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildForgeApp.Data;
using BuildForgeApp.Models;

namespace BuildForgeApp.Controllers
{
    [Authorize]
    public class BuildsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BuildsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var builds = await _context.Builds
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

            return View(builds);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            var build = await _context.Builds
                .Include(b => b.BuildComponents)
                .ThenInclude(bc => bc.PcComponent)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (build == null)
                return NotFound();

            build.TotalPrice = build.BuildComponents
                .Where(bc => bc.PcComponent != null)
                .Sum(bc => bc.PcComponent!.Price);
            var warnings = GetCompatibilityWarnings(build);
            ViewBag.Warnings = warnings;

            return View(build);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Build build)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                ModelState.AddModelError("", "You must be logged in to create a build.");
                return View(build);
            }
            if (string.IsNullOrWhiteSpace(build.BuildName))
            {
                ModelState.AddModelError("BuildName", "Build name is required.");
                return View(build);
            }
            try
            {
                build.UserId = userId;
                build.CreatedDate = DateTime.Now;
                build.TotalPrice = 0;

                _context.Builds.Add(build);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong while creating the build. Please try again.");
                return View(build);
            }
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            var build = await _context.Builds
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (build == null)
                return NotFound();

            return View(build);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string buildName)
        {
            var userId = _userManager.GetUserId(User);

            var build = await _context.Builds
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (build == null)
                return NotFound();

            build.BuildName = buildName;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            var build = await _context.Builds
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (build == null)
                return NotFound();

            return View(build);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveComponent(int buildId, int componentId)
        {
            var buildComponent = await _context.BuildComponents
                .FirstOrDefaultAsync(bc => bc.BuildId == buildId && bc.PcComponentId == componentId);

            if (buildComponent == null)
                return NotFound();

            var component = await _context.PcComponents
                .FirstOrDefaultAsync(c => c.Id == componentId);

            if (component != null)
            {
                var build = await _context.Builds
                    .FirstOrDefaultAsync(b => b.Id == buildId);

                if (build != null)
                {
                    build.TotalPrice -= component.Price;
                }
            }

            _context.BuildComponents.Remove(buildComponent);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Builds", new { id = buildId });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);

            var build = await _context.Builds
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (build == null)
                return NotFound();

            try
            {
                _context.Builds.Remove(build);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "This build could not be deleted. Please try again.");
                return View("Delete", build);
            }

            
        }

        // Checks compatibility and returns warning messages
        private List<string> GetCompatibilityWarnings(Build build)
        {
            var warnings = new List<string>();

            var components = build.BuildComponents
                .Where(bc => bc.PcComponent != null)
                .Select(bc => bc.PcComponent!)
                .ToList();

            var cpu = components.FirstOrDefault(c => c.ComponentType == "CPU");
            var motherboard = components.FirstOrDefault(c => c.ComponentType == "Motherboard");
            var psu = components.FirstOrDefault(c => c.ComponentType == "PSU");

            // CPu and Motherboard compatibility
            if (cpu != null && motherboard != null &&
                !string.IsNullOrEmpty(cpu.SocketType) &&
                !string.IsNullOrEmpty(motherboard.SocketType) &&
                cpu.SocketType != motherboard.SocketType)
            {
                warnings.Add($"CPU socket mismatch: {cpu.SocketType} vs {motherboard.SocketType}");
            }

            // PSU wattage check
            if (psu != null && psu.Wattage.HasValue)
            {
                int totalWattage = components
                    .Where(c => c.ComponentType != "PSU")
                    .Sum(c => c.Wattage ?? 0);

                if (totalWattage > psu.Wattage.Value)
                {
                    warnings.Add($"Power supply too weak. Required: {totalWattage}W, PSU: {psu.Wattage}W");
                }
            }

            return warnings;
        }
    }
}
