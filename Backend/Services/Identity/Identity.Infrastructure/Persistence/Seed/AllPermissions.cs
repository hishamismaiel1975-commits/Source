using Identity.Core.Persistence.Entities;
using Platform.Lib.Constants;

namespace Identity.Infrastructure.Persistence.Seed
{
    public static class AllPermissions
    {
        public static IEnumerable<Permission> GetPermissions => new List<Permission>
            {
                // Brand
                new Permission { Name = PermissionConstants.Brands.Create, NameEn = "Create Brand", NameAr = "إضافة علامة تجارية" },
                new Permission { Name = PermissionConstants.Brands.Update, NameEn = "Update Brand", NameAr = "تعديل علامة تجارية" },
                new Permission { Name = PermissionConstants.Brands.Delete, NameEn = "Delete Brand", NameAr = "حذف علامة تجارية" },

                // Type
                new Permission { Name = PermissionConstants.Types.Create, NameEn = "Create Type", NameAr = "إضافة نوع" },
                new Permission { Name = PermissionConstants.Types.Update, NameEn = "Update Type", NameAr = "تعديل نوع" },
                new Permission { Name = PermissionConstants.Types.Delete, NameEn = "Delete Type", NameAr = "حذف نوع" },

                // Product
                new Permission { Name = PermissionConstants.Products.Create, NameEn = "Create Product", NameAr = "إضافة منتج" },
                new Permission { Name = PermissionConstants.Products.Update, NameEn = "Update Product", NameAr = "تعديل منتج" },
                new Permission { Name = PermissionConstants.Products.Delete, NameEn = "Delete Product", NameAr = "حذف منتج" },

                // User
                new Permission { Name = PermissionConstants.Users.Read, NameEn = "Read Users", NameAr = "عرض المستخدمين" },
                new Permission { Name = PermissionConstants.Users.Create, NameEn = "Create User", NameAr = "إضافة مستخدم" },
                new Permission { Name = PermissionConstants.Users.Update, NameEn = "Update User", NameAr = "تعديل مستخدم" },
                new Permission { Name = PermissionConstants.Users.Delete, NameEn = "Delete User", NameAr = "حذف مستخدم" },

                // Role
                new Permission { Name = PermissionConstants.Roles.Read, NameEn = "Read Roles", NameAr = "عرض الأدوار" },
                new Permission { Name = PermissionConstants.Roles.Create, NameEn = "Create Role", NameAr = "إضافة دور" },
                new Permission { Name = PermissionConstants.Roles.Update, NameEn = "Update Role", NameAr = "تعديل دور" },
                new Permission { Name = PermissionConstants.Roles.Delete, NameEn = "Delete Role", NameAr = "حذف دور" },

                // Permission
                new Permission { Name = PermissionConstants.Permissions.Read, NameEn = "Read Permissions", NameAr = "عرض الصلاحيات" },
                new Permission { Name = PermissionConstants.Permissions.Assign, NameEn = "Assign Permissions", NameAr = "تعيين الصلاحيات" },

                 // File Storage
                new Permission { Name = PermissionConstants.FileStorage.Upload, NameEn = "Upload File", NameAr = "رفع ملف" },
                new Permission { Name = PermissionConstants.FileStorage.Read, NameEn = "Read File", NameAr = "عرض ملف" },
                new Permission { Name = PermissionConstants.FileStorage.Delete, NameEn = "Delete File", NameAr = "حذف ملف" },

                // Reporting    
                new Permission { Name = PermissionConstants.Reporting.Read, NameEn = "Read Reporting", NameAr = "عرض التقارير" },

            };
    }
}

