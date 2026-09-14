using Identity.Core.Enums;
using Identity.Core.Persistence.Entities;
using Microsoft.Extensions.DependencyInjection;
using Platform.Lib.Core.Persistence.Repositories;

namespace Identity.Infrastructure.Persistence.Seed
{
    public class DatabaseSeeder
    {
        // Add Built-in Roles, Users, and Permissions to the database
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleRepository = services.GetRequiredService<IRepository<Role>>();
            var userRepository = services.GetRequiredService<IRepository<User>>();
            var permissionRepository = services.GetRequiredService<IRepository<Permission>>();
            var rolePermissionRepository = services.GetRequiredService<IRepository<RolePermission>>();

            // --------------------------------------------------
            // Roles
            // --------------------------------------------------
            if (await roleRepository.CountAsync() == 0)
            {
                var existingRoles = new List<Role>
                    {
                        new Role
                        {
                            Name = "admin",
                            NameEn = "System Administrator",
                            NameAr = "مدير النظام",
                            IsBuiltIn= true
                        }

                    };
                await roleRepository.CreateManyAsync(existingRoles);

            }

            // --------------------------------------------------
            // Users
            // --------------------------------------------------
            var adminRole = await roleRepository.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (await userRepository.CountAsync() == 0)
            {

                var existingUsers = new List<User>
                    {
                        new User
                        {
                            UserName = "admin",
                            PasswordHash = "XXQGU2+HXrQO1eYDsSoZ02UhtxmCHPki4B/ybqbpUYM=",
                            NameEn = "Admin",
                            NameAr = "مدير",
                            UserType    = UserType.Employee,
                            IsBuiltIn= true,
                            IsActive=true,
                            RoleId= adminRole.Id
                        }
                    };

                await userRepository.CreateManyAsync(existingUsers);
            }

            // --------------------------------------------------
            // Permissions
            // --------------------------------------------------
            var permissions = AllPermissions.GetPermissions;

            // Get existing permissions
            var existingPermissions = await permissionRepository.GetAllAsync();

            // Find only new permissions
            var existingPermissionNames = existingPermissions
                .Select(x => x.Name)
                .ToHashSet();

            var newPermissions = permissions
                .Where(x => !existingPermissionNames.Contains(x.Name))
                .ToList();

            // Insert only missing permissions
            if (newPermissions.Any())
            {
                await permissionRepository.CreateManyAsync(newPermissions);

                // Refresh list so newly-created permissions have their IDs
                existingPermissions = await permissionRepository.GetAllAsync();
            }

            // --------------------------------------------------
            // Admin Role Permissions
            // --------------------------------------------------
            var rolePermissions = await rolePermissionRepository.GetAllAsync(x => x.RoleId == adminRole.Id);

            var existingAdminPermissionIds = rolePermissions
                .Select(x => x.PermissionId)
                .ToHashSet();

            var adminPermissions = existingPermissions
                .Where(x => !existingAdminPermissionIds.Contains(x.Id))
                .Select(x => new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = x.Id
                })
                .ToList();

            if (adminPermissions.Any())
            {
                await rolePermissionRepository.CreateManyAsync(adminPermissions);
            }
        }


    }




}
