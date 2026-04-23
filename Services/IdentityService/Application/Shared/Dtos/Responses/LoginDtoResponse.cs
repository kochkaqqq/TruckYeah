namespace Application.Shared.Dtos.Responses
{
    public class LoginDtoResponse
    {
        public string JwtToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
