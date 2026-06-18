namespace ListingService.WebApi.Contracts.Moderation;

public sealed class RejectListingRequest
{
    public string Reason { get; set; } = string.Empty;
}
