namespace SimpleECommerceBackend.Api.Authorization;

public static class AuthorizationPolicies
{
    public const string RequireAdminRole = "__SimpleECommerce_RequireAdminRole__";
    public const string RequireSellerRole = "__SimpleECommerce_RequireSellerRole__";
    public const string RequireCustomerRole = "__SimpleECommerce_RequireCustomerRole__";

}