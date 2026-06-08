namespace Application.Shared.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string EntityName { get; } = null!;
        public string EntityId { get; } = null!;

        public EntityNotFoundException(string entityName, string entityId)
            : base($"Entity '{entityName}' with ID '{entityId}' wasn`t found.")
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
