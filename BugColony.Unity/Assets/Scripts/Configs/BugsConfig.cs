using UnityEngine;


[CreateAssetMenu(menuName = "Configs/BugsConfig")]
public class BugsConfig : ScriptableObject
{
    [Header("Lifetimes")]
    public float WorkerLifetime = 30f;
    public float PredatorLifetime = 20f;

    [Header("Lifetime System")]
    public float DeadBugCleanupDelay = 5f;

    [Header("Mutation System")]
    public float MutationChance = 0.1f;
}


[CreateAssetMenu(menuName = "Configs/ColonyConfig")]
public class ColonyConfig : ScriptableObject
{
    public int MutationColonyThreshold = 10;
    public float MutationChance = 0.1f;
    public float SplitSpawnRadius = 1.5f;
}


[CreateAssetMenu(menuName = "Configs/ResourceSpawnerConfig")]
public class ResourceSpawnerConfig : ScriptableObject
{
    public float SpawnInterval = 2f;
    public Vector3 SpawnAreaMin = new(-20f, 0f, -20f);
    public Vector3 SpawnAreaMax = new(20f, 0f, 20f);
}


[CreateAssetMenu(menuName = "Configs/GameBootstrapConfig")]
public class GameBootstrapConfig : ScriptableObject
{
    public int InitialWorkerCount = 5;
    public int InitialPredatorCount = 2;
    public float InitialSpawnRangeMin = -10f;
    public float InitialSpawnRangeMax = 10f;
}