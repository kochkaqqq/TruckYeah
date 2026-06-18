using Domain.Enums;

namespace Application.Shared.Dtos.Responses;

public sealed class AdminUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public UserType UserType { get; set; }
    public AccountRole Role { get; set; }
    public AccountStatus Status { get; set; }
    public string? City { get; set; }
    public string? Company { get; set; }
    public string? AvatarLink { get; set; }
    public float Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}
