using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildForgeApp.Data;
using BuildForgeApp.Models;

namespace BuildForgeApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BuildCompatibilityApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BuildCompatibilityApiController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{buildId}")]
        public async Task<IActionResult> CheckCompatibility(int buildId)
        {
            var userId = _userManager.GetUserId(User);

            var build = await _context.Builds
                .Include(b => b.BuildComponents)
                .ThenInclude(bc => bc.PcComponent)
                .FirstOrDefaultAsync(b => b.Id == buildId && b.UserId == userId);

            if (build == null)
            {
                return NotFound();
            }

            var warnings = GetCompatibilityWarnings(build);

            return Ok(new
            {
                isCompatible = !warnings.Any(),
                warnings = warnings
            });
        }

        private List<string> GetCompatibilityWarnings(Build build)
        {
            var warnings = new List<string>();

            var components = build.BuildComponents
                .Where(bc => bc.PcComponent != null)
                .Select(bc => bc.PcComponent!)
                .ToList();

            var cpus = components.Where(c => c.ComponentType == "CPU").ToList();
            var motherboards = components.Where(c => c.ComponentType == "Motherboard").ToList();
            var psus = components.Where(c => c.ComponentType == "PSU").ToList();

            var cpu = cpus.FirstOrDefault();
            var motherboard = motherboards.FirstOrDefault();
            var psu = psus.FirstOrDefault();

            if (cpus.Count == 0)
            {
                warnings.Add("No CPU selected.");
            }

            if (cpus.Count > 1)
            {
                warnings.Add("Multiple CPUs selected. Only one CPU is allowed.");
            }

            if (motherboards.Count == 0)
            {
                warnings.Add("No motherboard selected.");
            }

            if (motherboards.Count > 1)
            {
                warnings.Add("Multiple motherboards selected. Only one motherboard is allowed.");
            }

            if (psus.Count == 0)
            {
                warnings.Add("No power supply selected.");
            }

            if (psus.Count > 1)
            {
                warnings.Add("Multiple power supplies selected. Only one PSU is allowed.");
            }

            if (cpu != null && motherboard != null &&
                !string.IsNullOrEmpty(cpu.SocketType) &&
                !string.IsNullOrEmpty(motherboard.SocketType) &&
                !cpu.SocketType.Equals(motherboard.SocketType, StringComparison.OrdinalIgnoreCase))
            {
                warnings.Add($"CPU socket mismatch: {cpu.SocketType} vs {motherboard.SocketType}.");
            }

            if (psu != null && psu.Wattage.HasValue)
            {
                int totalWattage = components
                    .Where(c => c.ComponentType != "PSU")
                    .Sum(c => c.Wattage ?? 0);

                if (totalWattage > psu.Wattage.Value)
                {
                    warnings.Add($"Power supply too weak. Required: {totalWattage}W, PSU: {psu.Wattage.Value}W.");
                }
            }

            return warnings;
        }
    }
}