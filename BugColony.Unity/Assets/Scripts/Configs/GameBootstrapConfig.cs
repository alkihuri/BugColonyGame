using UnityEngine;

[CreateAssetMenu(menuName = "Configs/GameBootstrapConfig")]
public class GameBootstrapConfig : ScriptableObject
{
    public int InitialWorkerCount = 5;
    public int InitialPredatorCount = 2;
    public float InitialSpawnRangeMin = -10f;
    public float InitialSpawnRangeMax = 10f;
}