using UnityEngine;

[CreateAssetMenu(menuName = "Configs/ResourceSpawnerConfig")]
public class ResourceSpawnerConfig : ScriptableObject
{
    public float SpawnInterval = 2f;
    public Vector3 SpawnAreaMin = new(-20f, 0f, -20f);
    public Vector3 SpawnAreaMax = new(20f, 0f, 20f);
}