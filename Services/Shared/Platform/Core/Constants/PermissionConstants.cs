using System.Reflection;

namespace Platform.Lib.Core.Constants
{
    public static class PermissionConstants
    {
        public static IEnumerable<string?> GetPermissions
        {
            get
            {
                var permissionTypes = typeof(PermissionConstants).GetNestedTypes();
                var permissionList = new List<string?>();
                foreach (var type in permissionTypes)
                {
                    var permissions = type
                        .GetFields(
                            BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.FlattenHierarchy)
                        .Where(x => x.FieldType == typeof(string))
                        .Select(x => x.GetValue(null)?.ToString())
                        .Where(x => !string.IsNullOrEmpty(x));

                    permissionList.AddRange(permissions);
                }

                return permissionList;
            }
        }
        public static class Brands
        {
            public const string Create = "Brand.Create";
            public const string Update = "Brand.Update";
            public const string Delete = "Brand.Delete";
        }
        public static class Types
        {
            public const string Create = "Type.Create";
            public const string Update = "Type.Update";
            public const string Delete = "Type.Delete";
        }
        public static class Products
        {
            public const string Create = "Product.Create";
            public const string Update = "Product.Update";
            public const string Delete = "Product.Delete";
        }
        public static class Users
        {
            public const string Read = "User.Read";
            public const string Create = "User.Create";
            public const string Update = "User.Update";
            public const string Delete = "User.Delete";
        }
        public static class Roles
        {
            public const string Read = "Role.Read";
            public const string Create = "Role.Create";
            public const string Update = "Role.Update";
            public const string Delete = "Role.Delete";
        }
        public static class Permissions
        {
            public const string Read = "Permission.Read";
            public const string Assign = "Permission.Assign";
        }

    }
}
