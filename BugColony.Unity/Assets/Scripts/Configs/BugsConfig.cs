using UnityEngine;


[CreateAssetMenu(menuName = "Configs/BugsConfig")]
public class BugsConfig : ScriptableObject
{
    public float WorkerLifetime = 30f;
    public float PredatorLifetime = 20f;
    
    
}


[CreateAssetMenu(menuName = "Configs/ColonyConfig")]
public class ColonyConfg : ScriptableObject
{ 
        public int MutationColonyThreshold = 10;
        public float MutationChance = 0.1f;
        public float SplitSpawnRadius = 1.5f;
}