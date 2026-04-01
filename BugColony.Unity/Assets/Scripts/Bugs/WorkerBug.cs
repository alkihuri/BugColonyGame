using BugColony.Bugs.Behaviors;

namespace BugColony.Bugs
{
    public class WorkerBug : BugBase
    {
        protected override void Awake()
        {
            base.Awake();
            SetBehavior(new WorkerBehavior());
        }
    }
}
