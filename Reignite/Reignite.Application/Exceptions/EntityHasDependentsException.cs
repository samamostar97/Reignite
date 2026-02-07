namespace Reignite.Application.Exceptions
{
    public class EntityHasDependentsException : ConflictException
    {
        public string EntityType { get; }
        public string DependentType { get; }
        public int DependentCount { get; }

        public EntityHasDependentsException(string entityType, string dependentType, int count)
            : base($"Ne možete obrisati {entityType} jer ima {count} {dependentType}.")
        {
            EntityType = entityType;
            DependentType = dependentType;
            DependentCount = count;
        }
    }
}
