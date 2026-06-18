namespace AdminService.Domain.Entities
{
    public class RefreshToken
    {
        public string TokenHash { get; set; } = null!;
        public Admin Admin { get; set; } = null!;
        public Guid AdminId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ExpireAt { get; set; }
        public DateTime RevokedAt { get; set; }
    }
}
