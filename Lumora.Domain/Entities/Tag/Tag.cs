using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Identity;

namespace Lumora.Domain.Entities.Tag;

public class Tag : BaseEntity
{
    public string Name { get; set; }  // Just the name, no TagType

    public virtual ICollection<StudioProfile> Studios { get; set; } = [];

    public Tag(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (name.Any(char.IsUpper))
            throw new ArgumentException("Name must be lowercase.", nameof(name));
        if (name.Contains(' '))
            throw new ArgumentException("Name cannot contain spaces.", nameof(name));
        if (!name.All(c => char.IsLetterOrDigit(c)))
            throw new ArgumentException("Name can only contain letters and digits.", nameof(name));

        Name = name;
    }
}