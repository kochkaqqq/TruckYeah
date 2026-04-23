namespace Application.Shared.Dtos.Requests
{
    public class LoginDtoRequest
    {
        public string Password { get; set; } = null!;
        public string DeviceId { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
