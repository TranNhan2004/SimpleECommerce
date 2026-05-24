namespace SimpleECommerceBackend.Domain.Constants.Uam;

public static class RoleCodes
{
    public const string Admin = "admin";
    public const string Seller = "seller";
    public const string Customer = "customer";

    public static readonly IReadOnlyList<string> All =
    [
        Admin,
        Seller,
        Customer
    ];
}
