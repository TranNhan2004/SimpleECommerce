namespace SimpleECommerceBackend.Domain.Constants.Permissions;

public static class PermissionCodes
{
    public const string UsersSelfCreate = "users.self.create";
    public const string UsersSelfRead = "users.self.read";
    public const string UsersSelfUpdate = "users.self.update";
    public const string UsersSelfActivate = "users.self.activate";
    public const string UsersSelfDelete = "users.self.delete";
    public const string PermissionsSelfRead = "permissions.self.read";
    public const string CategoriesRead = "categories.read";
    public const string CategoriesReadAdmin = "categories.read.admin";
    public const string CategoriesCreate = "categories.create";
    public const string CategoriesUpdate = "categories.update";
    public const string CategoriesDelete = "categories.delete";

    public static readonly IReadOnlyList<string> All =
    [
        UsersSelfCreate,
        UsersSelfRead,
        UsersSelfUpdate,
        UsersSelfActivate,
        UsersSelfDelete,
        PermissionsSelfRead,
        CategoriesRead,
        CategoriesReadAdmin,
        CategoriesCreate,
        CategoriesUpdate,
        CategoriesDelete
    ];
}
