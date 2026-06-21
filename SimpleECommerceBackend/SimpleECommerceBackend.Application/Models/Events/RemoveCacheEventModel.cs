namespace SimpleECommerceBackend.Application.Models.Events;

public class RemoveCacheEventModel
{
    public List<string> Keys { get; set; } = [];
    public List<string> PrefixKeys { get; set; } = [];
}