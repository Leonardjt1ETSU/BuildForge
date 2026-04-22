using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildForgeApp.Data;
using BuildForgeApp.Models;

namespace BuildForgeApp.Controllers
{
    public class PcComponentsController : Controller
    {
        //Handles the public component catalog
        private readonly ApplicationDbContext _context;
        
        public PcComponentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // SHows the catlog with category, search, and brand filtering
        public async Task<IActionResult> Index(string? category, string? searchString, string? brand)
        {
            var query = _context.PcComponents
                .Where(pc => pc.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(pc => pc.ComponentType == category);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(pc =>
                    pc.Name.Contains(searchString) ||
                    pc.Brand.Contains(searchString) ||
                    pc.ComponentType.Contains(searchString));
                
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(pc => pc.Brand == brand);
            }

            var components = await query
                .OrderBy(pc => pc.ComponentType)
                .ThenBy(pc => pc.Brand)
                .ThenBy(pc => pc.Name)
                .ToListAsync();
            
            var categories = await _context.PcComponents
                .Where(pc => pc.IsActive)
                .Select(pc => pc.ComponentType)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var brands = await _context.PcComponents
                .Where(pc => pc.IsActive)
                .Select(pc => pc.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedBrand = brand;

            return View(components);
        }

        //Shows full details for one component
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var component = await _context.PcComponents
                .FirstOrDefaultAsync(pc => pc.Id == id && pc.IsActive);

            if (component == null)
            {
                return NotFound();
            }

            return View(component);
        }



    }
}