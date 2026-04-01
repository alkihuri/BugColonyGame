using BugColony.Core;

namespace BugColony.Bugs.States
{
    public class DeadState : BugStateBase
    {
        public override void Enter(IBug bug)
        {
            // Bug has died - disable movement, play death animation, etc.
        }

        public override void Execute(IBug bug)
        {
            // No updates when dead
        }

        public override void Exit(IBug bug) { }
    }
}
