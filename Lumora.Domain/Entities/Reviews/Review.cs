using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Identity;

namespace Lumora.Domain.Entities.Reviews;

public class Review : BaseEntity
{
    public Guid StudioId { get; private set; }
    public Guid ConsumerId { get; private set; }
    public Guid InquiryId { get; private set; }
    public decimal Rating { get; private set; }

    public string? Title { get;  set; }
    public string? Comment { get; set; }

    //navigation property
    public virtual StudioProfile Studio { get; set; } = null!;
    public virtual ConsumerProfile Consumer { get; set; } = null!;
    public virtual Inquiry Inquiry { get; set; } = null!;

    public Review(Guid studioId, Guid consumerId, Guid inquiryId, decimal rating, string? title = null, string? comment = null)
    {
        if (studioId == Guid.Empty)
            throw new ArgumentException("Studio ID cannot be empty.", nameof(studioId));

        if (consumerId == Guid.Empty)
            throw new ArgumentException("Consumer ID cannot be empty.", nameof(consumerId));

        if (inquiryId == Guid.Empty)
            throw new ArgumentException("Inquiry ID cannot be empty.", nameof(inquiryId));

        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

        if ((title == null && comment != null) || (title != null && comment == null))
            throw new ArgumentException("Please provide both comment and title for the review", nameof(title)); 

        StudioId = studioId;
        ConsumerId = consumerId;
        InquiryId = inquiryId;
        Rating = rating;
        Title = title;
        Comment = comment;
    }
}
