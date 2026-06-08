using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string PasswordHash { get; set; } = null!;
        public Email Email { get; set; } = null!;
        public Phone Phone { get; set; } = null!;
        public Country Country { get; set; } = null!;
        public Postcode Postcode { get; set; } = null!;
        public UserType UserType { get; set; }
        public VatId VatId { get; set; } = null!;
        public FullName? FullName { get; set; } = null!;
        public Company? Company { get; set; }
        public string? AvatarLink { get; set; }
        public float Rating { get; set; }
    }
}
