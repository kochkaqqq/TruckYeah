namespace AdminService.Domain.Entities
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
