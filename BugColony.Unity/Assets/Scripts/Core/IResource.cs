namespace BugColony.Core
{
    public interface IResource : ISpawnable
    {
        float NutrientValue { get; }
        bool IsConsumed { get; }
        void Consume();
        
        string GetResourceType();
    }
}
