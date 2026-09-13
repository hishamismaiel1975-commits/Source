using Identity.Core.Persistence.Entities;
using Identity.Infrastructure.Persistence.Seed.Permissions;
using Microsoft.Extensions.DependencyInjection;
using Platform.Lib.Core.Persistence.Repositories;

namespace Identity.Infrastructure.Persistence.Seed
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleRepository = services.GetRequiredService<IRepository<Role>>();
            var userRepository = services.GetRequiredService<IRepository<User>>();
            var permissionRepository = services.GetRequiredService<IRepository<Permission>>();
            var rolePermissionRepository = services.GetRequiredService<IRepository<RolePermission>>();

            // --------------------------------------------------
            // Roles
            // --------------------------------------------------
            var existingRoles = await roleRepository.GetAllAsync();
            if (!existingRoles.Any())
            {
                existingRoles = new List<Role>
                    {
                        new Role
                        {
                            Name = "Admin",
                            NameEn = "System Administrator",
                            NameAr = "مدير النظام",
                            IsBuiltIn= true
                        },
                           new Role
                        {
                            Name = "Customer",
                            NameEn = "Customer",
                            NameAr = "عميل",
                            IsBuiltIn= true
                        },
                             new Role
                        {
                            Name = "Gust",
                            NameEn = "Guest",
                            NameAr = "زائر",
                            IsBuiltIn= true
                        }

                    };
                await roleRepository.CreateManyAsync(existingRoles);

            }

            // --------------------------------------------------
            // Users
            // --------------------------------------------------
            var existingUsers = await userRepository.GetAllAsync();
            if (!existingUsers.Any())
            {
                existingUsers = new List<User>
                    {
                        new User
                        {
                            UserName = "admin",
                            Password = "123",
                            NameEn = "Admin",
                            NameAr = "مدير",
                            IsBuiltIn= true
                        },
                        new User
                        {
                            UserName = "Guest",
                            Password = "123",
                            NameEn = "Guest",
                            NameAr = "زائر",
                            IsBuiltIn= true
                        },
                           new User
                        {
                            UserName = "Customer1",
                            Password = "123",
                            NameEn = "Customer1",
                            NameAr = "1 عميل",
                            IsBuiltIn= false
                        }
                    };

                await userRepository.CreateManyAsync(existingUsers);
            }

            // --------------------------------------------------
            // Permissions
            // --------------------------------------------------
            var permissions = AdminPermissions.GetPermissions;

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
            var adminRole = existingRoles.First(r => r.Name == "Admin");

            var rolePermissions = await rolePermissionRepository.GetAllAsync();

            var existingAdminPermissionIds = rolePermissions
                .Where(x => x.RoleId == adminRole.Id)
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
