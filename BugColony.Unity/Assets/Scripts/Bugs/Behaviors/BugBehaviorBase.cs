using BugColony.Core;

namespace BugColony.Bugs.Behaviors
{
    public abstract class BugBehaviorBase : IBugBehavior
    {
        protected IBug Bug { get; private set; }

        public virtual void Initialize(IBug bug)
        {
            Bug = bug;
        }

        public abstract void Execute(IBug bug);
    }
}
