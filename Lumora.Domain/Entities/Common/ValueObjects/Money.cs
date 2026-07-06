namespace Lumora.Domain.Entities.Common.ValueObjects;

public class Money : BaseEntity
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";

}
