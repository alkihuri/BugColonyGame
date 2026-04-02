using UnityEngine;
using BugColony.Bugs;

namespace BugColony.Systems
{
    public class MutationSystem
    {
        private readonly float _mutationChance;

        public MutationSystem(BugsConfig bugsConfig)
        {
            _mutationChance = bugsConfig.MutationChance;
        }

        public void TryMutate(BugBase bug)
        {
            if (Random.value > _mutationChance) return;
            ApplyRandomMutation(bug);
        }

        private void ApplyRandomMutation(BugBase bug)
        {
            // Placeholder: Mutations can modify speed, health, energy, etc.
            // This system is designed to be extended with specific mutation strategies.
            Debug.Log($"[MutationSystem] Mutation applied to {bug.name}");
        }
    }
}
