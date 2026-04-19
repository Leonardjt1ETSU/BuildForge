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

        // Handles build-related actions for logged-in users
        public BuildsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Shows only the current user's builds
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var builds = await _context.Builds
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

            return View(builds);
        }

        // Shows details for one build owned by the current user
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var build = await _context.Builds
                .Include(b => b.BuildComponents)
                .ThenInclude(bc => bc.PcComponent)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (build == null)
            {
                return NotFound();
            }

            return View(build);
        }

        // Displays the create build page
        public IActionResult Create()
        {
            return View();
        }

        // Creates a new build for the logged-in user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BuildName")] Build build)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            if (ModelState.IsValid)
            {
                build.UserId = userId;
                build.CreatedDate = DateTime.Now;
                build.TotalPrice = 0;

                _context.Add(build);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(build);
        }

        // Displays the edit page for a user's build
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var build = await _context.Builds
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (build == null)
            {
                return NotFound();
            }

            return View(build);
        }

        // Saves changes to a user's build
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BuildName")] Build build)
        {
            if (id != build.Id)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var existingBuild = await _context.Builds
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (existingBuild == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingBuild.BuildName = build.BuildName;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(build);
        }

        // Displays the delete confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var build = await _context.Builds
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (build == null)
            {
                return NotFound();
            }

            return View(build);
        }

        // Deletes a user's build
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);

            var build = await _context.Builds
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (build == null)
            {
                return NotFound();
            }

            _context.Builds.Remove(build);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}