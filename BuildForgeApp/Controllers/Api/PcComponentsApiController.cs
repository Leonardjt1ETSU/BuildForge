using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildForgeApp.Data;
using BuildForgeApp.Models;

namespace BuildForgeApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PcComponentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PcComponentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PcComponent>>> GetComponents()
        {
            return await _context.PcComponents
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PcComponent>> GetComponent(int id)
        {
            var component = await _context.PcComponents.FindAsync(id);

            if (component == null || !component.IsActive)
            {
                return NotFound();
            }

            return component;
        }

        [HttpPost]
        public async Task<ActionResult<PcComponent>> CreateComponent(PcComponent component)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PcComponents.Add(component);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComponent), new { id = component.Id }, component);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComponent(int id, PcComponent component)
        {
            if (id != component.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(component).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            var component = await _context.PcComponents.FindAsync(id);

            if (component == null)
            {
                return NotFound();
            }

            component.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        
    }
}