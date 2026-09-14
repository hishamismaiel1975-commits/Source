namespace Platform.Lib.Core.Authorization
{
    public static class PermissionConstants
    {
        public static class Brand
        {
            public const string Create = "Brand.Create";
            public const string Update = "Brand.Update";
            public const string Delete = "Brand.Delete";
        }

        public static class Type
        {
            public const string Create = "Type.Create";
            public const string Update = "Type.Update";
            public const string Delete = "Type.Delete";
        }

        public static class Product
        {
            public const string Create = "Product.Create";
            public const string Update = "Product.Update";
            public const string Delete = "Product.Delete";
        }

        public static class User
        {
            public const string Read = "User.Read";
            public const string Create = "User.Create";
            public const string Update = "User.Update";
            public const string Delete = "User.Delete";
        }

        public static class Role
        {
            public const string Read = "Role.Read";
            public const string Create = "Role.Create";
            public const string Update = "Role.Update";
            public const string Delete = "Role.Delete";
        }

        public static class Permission
        {
            public const string Read = "Permission.Read";
            public const string Assign = "Permission.Assign";
        }

    }
}
