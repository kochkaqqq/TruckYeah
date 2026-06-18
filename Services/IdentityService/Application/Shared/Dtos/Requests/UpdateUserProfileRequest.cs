namespace Application.Shared.Dtos.Requests;

public sealed class UpdateUserProfileRequest
{
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Postcode { get; set; } = null!;
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? MiddleName { get; set; }
    public string? City { get; set; }
    public string? CompanyName { get; set; }
    public string? AvatarLink { get; set; }
}
