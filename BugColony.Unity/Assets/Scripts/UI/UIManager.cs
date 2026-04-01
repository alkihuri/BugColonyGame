using UnityEngine;
using UnityEngine.UI;
using BugColony.Systems;

namespace BugColony.UI
{
    public class UIManager
    {
        private readonly ColonyManager _colonyManager;
        private Text _aliveCountText;
        private Text _deadCountText;

        public UIManager(ColonyManager colonyManager)
        {
            _colonyManager = colonyManager;
        }

        public void BindUI(Text aliveCountText, Text deadCountText)
        {
            _aliveCountText = aliveCountText;
            _deadCountText = deadCountText;
        }

        public void Update()
        {
            if (_aliveCountText != null)
                _aliveCountText.text = $"Alive: {_colonyManager.TotalAlive}";

            if (_deadCountText != null)
                _deadCountText.text = $"Dead: {_colonyManager.TotalDead}";
        }
    }
}
