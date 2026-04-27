using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildForgeApp.Data;
using BuildForgeApp.Models;

namespace BuildForgeApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var components = await _context.PcComponents
                .OrderBy(c => c.ComponentType)
                .ThenBy(c => c.Brand)
                .ThenBy(c => c.Name)
                .ToListAsync();

            return View(components);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PcComponent component)
        {
            if (!ModelState.IsValid)
            {
                return View(component);
            }

            try
            {
                component.IsActive = true;

                _context.PcComponents.Add(component);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Unable to create component. Please try again.");
                return View(component);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var component = await _context.PcComponents.FindAsync(id);

            if (component == null)
                return NotFound();

            return View(component);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PcComponent component)
        {
            if (id != component.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View(component);
            }

            try
            {
                _context.Update(component);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Unable to update component. Please try again.");
                return View(component);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var component = await _context.PcComponents.FindAsync(id);

            if (component == null)
                return NotFound();

            return View(component);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var component = await _context.PcComponents.FindAsync(id);

            if (component == null)
                return NotFound();

            try
            {
                component.IsActive = false;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Unable to deactivate component. Please try again.");
                return View("Delete", component);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(int id)
        {
            var component = await _context.PcComponents.FindAsync(id);

            if (component == null)
                return NotFound();

            component.IsActive = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var component = await _context.PcComponents.FindAsync(id);

            if (component == null)
                return NotFound();

            try
            {
                _context.PcComponents.Remove(component);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Unable to delete component. Please try again.");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}