using Identity.Core.Persistence.Entities;
using Platform.Lib.Core.Authorization;

namespace Identity.Infrastructure.Persistence.Seed.Permissions
{
    public static class CustomerPermissions
    {
        public static IEnumerable<Permission> GetPermissions => new List<Permission>
            {
                // Brand
                new Permission { Name = AllPermissions.Brand.Read, NameEn = "Read Brands", NameAr = "عرض العلامات التجارية" },

                // Type
                new Permission { Name = AllPermissions.Type.Read, NameEn = "Read Types", NameAr = "عرض الأنواع" },

                // Product
                new Permission { Name = AllPermissions.Product.Read, NameEn = "Read Products", NameAr = "عرض المنتجات" },

            };
    }
}

