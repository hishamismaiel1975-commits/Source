namespace Platform.Lib.Core.Authorization
{
    public static class Permissions
    {
        public static class Users
        {
            public const string Read = "Users.Read";
            public const string Create = "Users.Create";
            public const string Update = "Users.Update";
            public const string Delete = "Users.Delete";
        }

        public static class Roles
        {
            public const string Read = "Roles.Read";
            public const string Create = "Roles.Create";
            public const string Update = "Roles.Update";
            public const string Delete = "Roles.Delete";
        }

        public static class PermissionsManagement
        {
            public const string Read = "Permissions.Read";
            public const string Assign = "Permissions.Assign";
        }

    }
}
