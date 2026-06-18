namespace Application.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "identity-service";
    public string Key { get; set; } = null!;
    public int AccessTokenMinutes { get; set; } = 30;
}
