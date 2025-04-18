using Microsoft.EntityFrameworkCore;
using PermissionsApp.Domain.Entities;

namespace PermissionsApp.Infraestructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<PermissionType> PermissionTypes { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PermissionType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Description).IsRequired();

            });

            modelBuilder.Entity<Permission>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.EmployeeName).IsRequired();
                entity.Property(e => e.EmployeeLastName).IsRequired();
                entity.Property(e => e.Date).IsRequired();

                entity.HasOne(p => p.PermissionType);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
