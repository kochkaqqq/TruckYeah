namespace RouteService.Domain.Exceptions;

public class RouteProviderUnavailableException : Exception
{
    public RouteProviderUnavailableException(string message)
        : base(message)
    {
    }

    public RouteProviderUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
