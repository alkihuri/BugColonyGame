namespace BugColony.Core
{
    public interface IBugBehavior
    {
        void Initialize(IBug bug);
        void Execute(IBug bug);
    }
}
