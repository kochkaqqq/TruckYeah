namespace Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public string PropertyName { get; } = null!;

        public ValidationException(string propertyName, string message)
            : base($"Invalid data for property '{propertyName}': {message}")
        {
            PropertyName = propertyName;
        }
    }
}
