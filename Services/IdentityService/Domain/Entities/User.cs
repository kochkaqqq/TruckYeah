using Domain.ValueObjects;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string PasswordHash { get; set; } = null!;
        public Email Email { get; set; } = null!;
        public Phone Phone { get; set; } = null!;
        public FullName FullName { get; set; } = null!;
        public Company? Company { get; set; }
    }
}
