using Domain.Enums;

namespace Application.Shared.Dtos.Requests
{
    public class UpdateUserRequestDto
    {
        public Guid Id { get; set; } = default!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Guid? CountryId { get; set; }
        public string? Postcode { get; set; }
        public UserType UserType { get; set; }
        public string? VatId { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; init; }
        public string? MiddleName { get; init; }
        public Guid? CompanyId { get; set; }
        public string? AvatarLink { get; set; }
        public float? Rating { get; set; }
    }
}
