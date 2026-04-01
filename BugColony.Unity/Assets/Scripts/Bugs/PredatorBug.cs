using BugColony.Bugs.Behaviors;

namespace BugColony.Bugs
{
    public class PredatorBug : BugBase
    {
        protected override void Awake()
        {
            base.Awake();
            SetBehavior(new PredatorBehavior());
        }
    }
}
