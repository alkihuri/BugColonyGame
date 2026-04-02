using UnityEngine;

[CreateAssetMenu(menuName = "Configs/ColonyConfig")]
public class ColonyConfig : ScriptableObject
{
    public int MutationColonyThreshold = 10;
    public float MutationChance = 0.1f;
    public float SplitSpawnRadius = 1.5f;
}