using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UHO_API.Core.Entities;


namespace UHO_API.Infraestructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    public DbSet<AreaModel> Areas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AreaModel>(area =>
        {
            area.HasOne(a => a.JefeArea)
                .WithOne()
                .HasForeignKey<AreaModel>(a=> a.JefeAreaId)
                .OnDelete(DeleteBehavior.SetNull);
            area.HasIndex(a => a.JefeAreaId).IsUnique();
        });
    }
}