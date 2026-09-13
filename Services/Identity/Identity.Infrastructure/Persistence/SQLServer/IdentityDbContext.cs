using Identity.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.SQLServer
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Role> Roles => Set<Role>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                        .HasOne(u => u.Role)
                        .WithMany(r => r.Users)
                        .HasForeignKey(u => u.RoleId)
                        .OnDelete(DeleteBehavior.Restrict);
        }
    }

}



