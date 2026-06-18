namespace Domain.Entities
{
    public class RefreshToken
    {
        public string TokenHash { get; set; } = null!;
        public User User { get; set; } = null!;
        public Guid UserId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ExpireAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string DeviceId { get; set; } = null!;    
        public string UserAgent { get; set; } = null!;   
        public string IpAddress { get; set; } = null!;   
    }
}
