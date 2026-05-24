namespace SimpleECommerceBackend.Domain.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class DisplayValueAttribute : Attribute
{
    public DisplayValueAttribute(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
