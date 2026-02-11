namespace SimpleECommerceBackend.Application.Interfaces.Services;

public interface IAddressService
{
    public bool IsValidAddress(string address);
    public bool IsValidWard(string province, string ward);
    public bool IsValidProvince(string province);
}