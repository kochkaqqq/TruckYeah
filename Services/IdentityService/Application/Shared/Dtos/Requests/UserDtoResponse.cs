using Domain.Enums;

namespace Application.Shared.Dtos.Requests
{
    public class UserDtoResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Postcode { get; set; } = null!;
        public UserType UserType { get; set; }
        public string VatId { get; set; } = null!;
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? AvatarLink { get; set; }
        public float Rating { get; set; }
    }
}
