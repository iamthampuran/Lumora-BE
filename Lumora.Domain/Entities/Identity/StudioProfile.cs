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
    public virtual ICollection<StudioTag> Tags { get; set; } = [];
    public virtual ICollection<Inquiry> Inquiries { get; set; } = [];
    public virtual ICollection<Payment> Payments { get; set; } = [];
    public virtual ICollection<Review> Reviews { get; set; } = [];
    public virtual ICollection<PortfolioImage> PortfolioImages { get; set; } = [];

    public ProfileCompletionResult GetProfileCompletion()
    {
        var steps = new List<ProfileCompletionStep>
        {
            new("Upload Studio Logo",        "Upload your studio logo to build recognition and trust.",  LogoUrl != null),
            new("Upload Cover Image",        "Showcase your brand with a beautiful cover photo.",        CoverImageUrl != null),
            new("Add Portfolio Photos",      "Add your best work and highlight your style.",             PortfolioImages.Count > 0),
            new("Add Photography Styles",    "Select the styles and genres you specialize in.",          Tags.Count > 0),
            new("Add Team Members",          "Invite your team and collaborate on projects.",            Employees.Count > 0),
            new("Set Service Area",          "Define the locations and radius you serve.",               Location != null && ServiceRadius != null),
        };

        var completedCount = steps.Count(s => s.IsCompleted);
        var percentage = (int)Math.Round((double)completedCount / steps.Count * 100);

        return new ProfileCompletionResult(percentage, steps);
    }

    //Methods
    public void UpdateLocation (Coordinates newLocation) => Location = newLocation;
    public void UpdateServiceRadius(ServiceRadius newServiceRadius) => ServiceRadius = newServiceRadius;
}

public record ProfileCompletionStep(string Title, string Description, bool IsCompleted);
public record ProfileCompletionResult(int Percentage, IReadOnlyList<ProfileCompletionStep> Steps);