using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BuildForgeApp.Models
{ // Represents a user-created PC build
    public class Build
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string BuildName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public decimal TotalPrice { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }

        public ICollection<BuildComponent> BuildComponents { get; set; } = new List<BuildComponent>();
    }
}