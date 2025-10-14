namespace Messaging.Contracts;

public record ContractFailed
{
    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    public required string Reason { get; init; }
    public int StatusCode { get; init; } = 400;
}