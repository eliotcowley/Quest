using UnityEngine;
using System.Collections;

public class SpawnableObjectList : MonoBehaviour
{
    private SpawnObjects spawnObjects;

    public static GameObject coinPrefab;
    public static GameObject ghostPrefab;
    public static SpawnableObject[] objects;
    public static GameObject heartPrefab;

    [HideInInspector]
    public static bool prefabsSet = false;

    public void Start()
    {
        Invoke("SetPrefabs", 1f);
    }

    private void SetPrefabs()
    {
        if (!prefabsSet)
        {
            prefabsSet = true;
        }

        spawnObjects = GetComponent<SpawnObjects>();
        coinPrefab = spawnObjects.coinPrefab;
        ghostPrefab = spawnObjects.ghostPrefab;
        heartPrefab = spawnObjects.heartPrefab;

        objects = new[]
        {
            // beatNum, prefab, trackNum
            new SpawnableObject(0, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(2, coinPrefab, 1),
            new SpawnableObject(2, coinPrefab, 0),
            new SpawnableObject(2, coinPrefab, 1),
            new SpawnableObject(2, coinPrefab, 2),
            new SpawnableObject(2, coinPrefab, 1),
            new SpawnableObject(2, coinPrefab, 0),
            new SpawnableObject(2, coinPrefab, 1),
            new SpawnableObject(2, coinPrefab, 2),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 0),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 2),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 0),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 2),
            new SpawnableObject(2, ghostPrefab, 1),
            new SpawnableObject(4, ghostPrefab, 0),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 0),
            new SpawnableObject(2, ghostPrefab, 2),
            new SpawnableObject(2, heartPrefab, 1)
        };
    }
}