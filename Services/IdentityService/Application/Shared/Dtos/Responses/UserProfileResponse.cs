using Domain.Enums;

namespace Application.Shared.Dtos.Responses;

public sealed class UserProfileResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? MiddleName { get; set; }
    public string? FullName { get; set; }
    public UserType UserType { get; set; }
    public Guid CountryId { get; set; }
    public string Country { get; set; } = null!;
    public string Postcode { get; set; } = null!;
    public string? City { get; set; }
    public Guid? CompanyId { get; set; }
    public string? Company { get; set; }
    public string? VatId { get; set; }
    public string? AvatarLink { get; set; }
    public float Rating { get; set; }
    public bool IsProfileCompleted { get; set; }
}
