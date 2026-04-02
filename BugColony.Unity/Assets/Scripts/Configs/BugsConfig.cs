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