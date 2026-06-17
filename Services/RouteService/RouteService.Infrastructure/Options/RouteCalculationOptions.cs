namespace RouteService.Infrastructure.Options;

public class RouteCalculationOptions
{
    public double DefaultFuelConsumptionLitersPer100Km { get; set; } = 35;
    public int RequestTimeoutSeconds { get; set; } = 10;
    public int CacheTtlHours { get; set; } = 24;
}
