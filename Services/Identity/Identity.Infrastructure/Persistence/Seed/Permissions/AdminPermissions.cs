using Identity.Core.Persistence.Entities;
using Platform.Lib.Core.Authorization;

namespace Identity.Infrastructure.Persistence.Seed.Permissions
{
    public static class AdminPermissions
    {
        public static IEnumerable<Permission> GetPermissions => new List<Permission>
            {
                // Brand
                new Permission { Name = AllPermissions.Brand.Read, NameEn = "Read Brands", NameAr = "عرض العلامات التجارية" },
                new Permission { Name = AllPermissions.Brand.Create, NameEn = "Create Brand", NameAr = "إضافة علامة تجارية" },
                new Permission { Name = AllPermissions.Brand.Update, NameEn = "Update Brand", NameAr = "تعديل علامة تجارية" },
                new Permission { Name = AllPermissions.Brand.Delete, NameEn = "Delete Brand", NameAr = "حذف علامة تجارية" },

                // Type
                new Permission { Name = AllPermissions.Type.Read, NameEn = "Read Types", NameAr = "عرض الأنواع" },
                new Permission { Name = AllPermissions.Type.Create, NameEn = "Create Type", NameAr = "إضافة نوع" },
                new Permission { Name = AllPermissions.Type.Update, NameEn = "Update Type", NameAr = "تعديل نوع" },
                new Permission { Name = AllPermissions.Type.Delete, NameEn = "Delete Type", NameAr = "حذف نوع" },

                // Product
                new Permission { Name = AllPermissions.Product.Read, NameEn = "Read Products", NameAr = "عرض المنتجات" },
                new Permission { Name = AllPermissions.Product.Create, NameEn = "Create Product", NameAr = "إضافة منتج" },
                new Permission { Name = AllPermissions.Product.Update, NameEn = "Update Product", NameAr = "تعديل منتج" },
                new Permission { Name = AllPermissions.Product.Delete, NameEn = "Delete Product", NameAr = "حذف منتج" },

                // User
                new Permission { Name = AllPermissions.User.Read, NameEn = "Read Users", NameAr = "عرض المستخدمين" },
                new Permission { Name = AllPermissions.User.Create, NameEn = "Create User", NameAr = "إضافة مستخدم" },
                new Permission { Name = AllPermissions.User.Update, NameEn = "Update User", NameAr = "تعديل مستخدم" },
                new Permission { Name = AllPermissions.User.Delete, NameEn = "Delete User", NameAr = "حذف مستخدم" },

                // Role
                new Permission { Name = AllPermissions.Role.Read, NameEn = "Read Roles", NameAr = "عرض الأدوار" },
                new Permission { Name = AllPermissions.Role.Create, NameEn = "Create Role", NameAr = "إضافة دور" },
                new Permission { Name = AllPermissions.Role.Update, NameEn = "Update Role", NameAr = "تعديل دور" },
                new Permission { Name = AllPermissions.Role.Delete, NameEn = "Delete Role", NameAr = "حذف دور" },

                // Permission
                new Permission { Name = AllPermissions.Permission.Read, NameEn = "Read Permissions", NameAr = "عرض الصلاحيات" },
                new Permission { Name = AllPermissions.Permission.Assign, NameEn = "Assign Permissions", NameAr = "تعيين الصلاحيات" }

            };
    }
}

