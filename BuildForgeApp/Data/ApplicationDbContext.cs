using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BuildForgeApp.Models;

namespace BuildForgeApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PcComponent> PcComponents { get; set; } = default!;
        public DbSet<Build> Builds { get; set; } = default!;
        public DbSet<BuildComponent> BuildComponents { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Build>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BuildComponent>()
                .HasOne(bc => bc.Build)
                .WithMany(b => b.BuildComponents)
                .HasForeignKey(bc => bc.BuildId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BuildComponent>()
                .HasOne(bc => bc.PcComponent)
                .WithMany(pc => pc.BuildComponents)
                .HasForeignKey(bc => bc.PcComponentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}