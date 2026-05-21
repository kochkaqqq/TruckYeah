using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Country
    {
        public Guid Id { get; set; } = default!;
        public CountryName Name { get; set; } = null!;
    }
}
