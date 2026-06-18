using Domain.Enums;

namespace Application.Shared.Dtos.Responses;

public sealed class PublicUserResponse
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public UserType UserType { get; set; }
    public string? City { get; set; }
    public string? Company { get; set; }
    public string? AvatarLink { get; set; }
    public float Rating { get; set; }
}
