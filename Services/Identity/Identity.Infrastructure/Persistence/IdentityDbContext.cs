using Identity.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Role> Roles => Set<Role>();

        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>(builder =>
            {
                // UserName unique index
                builder.HasIndex(u => u.UserName)
                       .IsUnique()
                       .HasDatabaseName("IX_Users_UserName");

                // User → Role
                builder.HasOne(u => u.Role)
                       .WithMany(r => r.Users)
                       .HasForeignKey(u => u.RoleId)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Role>(builder =>
            {
                // Role Name unique index
                builder.HasIndex(r => r.Name)
                       .IsUnique()
                       .HasDatabaseName("IX_Roles_Name");

            });

            modelBuilder.Entity<RolePermission>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.HasOne(x => x.Role)
                    .WithMany(x => x.RolePermissions)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.Permission)
                    .WithMany(x => x.RolePermissions)
                    .HasForeignKey(x => x.PermissionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Prevent duplicate Role + Permission assignments
                builder.HasIndex(x => new { x.RoleId, x.PermissionId })
                    .IsUnique();
            });
        }
    }

}



