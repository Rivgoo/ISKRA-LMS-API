namespace Iskra.Modules.Iam.Abstractions;

/// <summary>
/// Defines the comprehensive list of system permissions.
/// Organized hierarchically by feature area.
/// </summary>
public static class IskraPermissions
{
    public const string SystemFullAccess = "*";

    public static class Users
    {
        public const string Read = "users.read";
        public const string ReadDetail = "users.read.detail";
        public const string Create = "users.create";
        public const string Update = "users.update";
        public const string Delete = "users.delete";
        public const string ChangePassword = "users.change_password";
        public const string ManageStatus = "users.manage_status";
    }

    public static class Roles
    {
        public const string Read = "roles.read";
        public const string Create = "roles.create";
        public const string Update = "roles.update";
        public const string Delete = "roles.delete";
        public const string ManageAssignments = "roles.manage_assignments";
    }

    public static class Permissions
    {
        public const string Read = "permissions.read";
        public const string Manage = "permissions.manage";
    }
}