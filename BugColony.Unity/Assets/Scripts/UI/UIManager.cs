using BugColony.Systems;

namespace BugColony.UI
{
    public class UIManager
    {
        private readonly ColonyManager _colonyManager;
        private readonly UiController _ui;

        public UIManager(ColonyManager colonyManager, UiController ui)
        {
            _colonyManager = colonyManager;
            _ui = ui;
        }

        public void Update()
        {
            if (_ui == null) return;

            if (_ui.aliveCountText != null)
                _ui.aliveCountText.text = $"Alive Bugs: {_colonyManager.TotalAlive}";

            int deadWorkers = 0;
            int deadPredators = 0;

            foreach (var dead in _colonyManager.DeadBugs)
            {
                if (dead is BugColony.Bugs.PredatorBug) deadPredators++;
                else deadWorkers++;
            }

            if (_ui.deadWorkerCountText != null)
                _ui.deadWorkerCountText.text = $"Dead Workers: {deadWorkers}";

            if (_ui.deadPredatorCountText != null)
                _ui.deadPredatorCountText.text = $"Dead Predators: {deadPredators}";
        }
    }
}