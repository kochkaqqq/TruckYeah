namespace Application.Shared.Exceptions
{
    public class ConflictException : Exception
    {
        public string EntityName { get; }
        public string ConflictValue { get; }

        public ConflictException(string entityName, string conflictValue)
            : base($"{entityName} with value '{conflictValue}' already exists.")
        {
            EntityName = entityName;
            ConflictValue = conflictValue;
        }
    }
}
