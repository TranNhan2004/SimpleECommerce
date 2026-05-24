using SimpleECommerceBackend.Domain.Entities.Abstracts;

namespace SimpleECommerceBackend.Domain.Entities.Uam;

public class Role : EntityBase, ICreatedTrackable
{
    private string _code = null!;
    private string _name = null!;

    public string Code
    {
        get => _code;
        set => _code = value ?? throw new ArgumentNullException(nameof(Code));
    }

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(Name));
    }

    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; private set; }
}
