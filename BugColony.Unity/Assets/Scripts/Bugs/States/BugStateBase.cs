using BugColony.Core;

namespace BugColony.Bugs.States
{
    public abstract class BugStateBase : IBugState
    {
        public abstract void Enter(IBug bug);
        public abstract void Execute(IBug bug);
        public abstract void Exit(IBug bug);
    }
}
