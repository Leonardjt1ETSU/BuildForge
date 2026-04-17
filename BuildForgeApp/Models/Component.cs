using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BuildForgeApp.Models
{
    public class Component
    {
        public int Id { get; set; }

        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public String Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ComponentType { get; set; } = string.Empty;

        [Range(0.01, 1000.00)]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string? SocketType { get; set; }

        [StringLength(50)]
        public string? FormFactor { get; set; }

        [StringLength(50)]
        public int? Wattage { get; set; }

        public int? CapacityGB { get; set; }

        public bool IsActive { get; set; } = true;


    }
}