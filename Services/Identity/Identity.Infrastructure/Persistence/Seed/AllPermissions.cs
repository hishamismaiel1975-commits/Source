using Identity.Core.Persistence.Entities;
using Platform.Lib.Core.Authorization;

namespace Identity.Infrastructure.Persistence.Seed
{
    public static class AllPermissions
    {
        public static IEnumerable<Permission> GetPermissions => new List<Permission>
            {
                // Brand
                new Permission { Name = PermissionConstants.Brand.Create, NameEn = "Create Brand", NameAr = "إضافة علامة تجارية" },
                new Permission { Name = PermissionConstants.Brand.Update, NameEn = "Update Brand", NameAr = "تعديل علامة تجارية" },
                new Permission { Name = PermissionConstants.Brand.Delete, NameEn = "Delete Brand", NameAr = "حذف علامة تجارية" },

                // Type
                new Permission { Name = PermissionConstants.Type.Create, NameEn = "Create Type", NameAr = "إضافة نوع" },
                new Permission { Name = PermissionConstants.Type.Update, NameEn = "Update Type", NameAr = "تعديل نوع" },
                new Permission { Name = PermissionConstants.Type.Delete, NameEn = "Delete Type", NameAr = "حذف نوع" },

                // Product
                new Permission { Name = PermissionConstants.Product.Create, NameEn = "Create Product", NameAr = "إضافة منتج" },
                new Permission { Name = PermissionConstants.Product.Update, NameEn = "Update Product", NameAr = "تعديل منتج" },
                new Permission { Name = PermissionConstants.Product.Delete, NameEn = "Delete Product", NameAr = "حذف منتج" },

                // User
                new Permission { Name = PermissionConstants.User.Read, NameEn = "Read Users", NameAr = "عرض المستخدمين" },
                new Permission { Name = PermissionConstants.User.Create, NameEn = "Create User", NameAr = "إضافة مستخدم" },
                new Permission { Name = PermissionConstants.User.Update, NameEn = "Update User", NameAr = "تعديل مستخدم" },
                new Permission { Name = PermissionConstants.User.Delete, NameEn = "Delete User", NameAr = "حذف مستخدم" },

                // Role
                new Permission { Name = PermissionConstants.Role.Read, NameEn = "Read Roles", NameAr = "عرض الأدوار" },
                new Permission { Name = PermissionConstants.Role.Create, NameEn = "Create Role", NameAr = "إضافة دور" },
                new Permission { Name = PermissionConstants.Role.Update, NameEn = "Update Role", NameAr = "تعديل دور" },
                new Permission { Name = PermissionConstants.Role.Delete, NameEn = "Delete Role", NameAr = "حذف دور" },

                // Permission
                new Permission { Name = PermissionConstants.Permission.Read, NameEn = "Read Permissions", NameAr = "عرض الصلاحيات" },
                new Permission { Name = PermissionConstants.Permission.Assign, NameEn = "Assign Permissions", NameAr = "تعيين الصلاحيات" }

            };
    }
}

