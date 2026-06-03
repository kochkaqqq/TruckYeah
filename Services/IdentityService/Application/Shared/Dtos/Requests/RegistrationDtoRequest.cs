using Domain.Enums;

namespace Application.Shared.Dtos.Requests
{
    public class RegistrationDtoRequest
    {
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string DeviceId { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public Guid CountryId { get; set; } = default!;
        public string Postcode { get; set; } = null!;
        public UserType UserType { get; set; }
        public string VatId { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public string? Surname { get; set; } = null!;
        public string? MiddleName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? AvatarLink { get; set; }
    }
}
