using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Common.ValueObjects;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Payments;
using Lumora.Domain.Entities.Reviews;
using Lumora.Domain.Entities.Studio;

namespace Lumora.Domain.Entities.Identity;

public class StudioProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string StudioName { get; set; } = null!;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string Phone { get; set; } = null!;
    public string? Website {  get; set; }

    public Coordinates Location { get; set; } = null!;
    public ServiceRadius ServiceRadius { get; set; } = null!;

    public decimal StartingPrice { get; set; }
    public decimal? AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }

    //navigation
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Employee> Employees { get; set; } = [];
    public virtual ICollection<Tag.Tag> Tags { get; set; } = [];
    public virtual ICollection<Inquiry> Inquiries { get; set; } = [];
    public virtual ICollection<Payment> Payments { get; set; } = [];
    public virtual ICollection<Review> Reviews { get; set; } = [];
    public virtual ICollection<PortfolioImage> PortfolioImages { get; set; } = [];



    //Methods
    public void UpdateLocation (Coordinates newLocation) => Location = newLocation;
    public void UpdateServiceRadius(ServiceRadius newServiceRadius) => ServiceRadius = newServiceRadius;
}
