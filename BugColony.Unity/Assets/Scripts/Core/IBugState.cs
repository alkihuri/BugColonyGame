namespace BugColony.Core
{
    public interface IBugState
    {
        void Enter(IBug bug);
        void Execute(IBug bug);
        void Exit(IBug bug);
    }
}
