using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Company
    {
        public Guid Id { get; set; }
        public CompanyName Name { get; set; } = null!;
    }
}
