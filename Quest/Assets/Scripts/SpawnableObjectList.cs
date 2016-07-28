using UnityEngine;
using System.Collections;

public class SpawnableObjectList : MonoBehaviour
{
    private SpawnObjects spawnObjects;

    public static GameObject coinPrefab;
    public static GameObject ghostPrefab;
    public static SpawnableObject[] objects;

    public void Start()
    {
        Invoke("SetPrefabs", 1f);
    }

    private void SetPrefabs()
    {
        spawnObjects = GetComponent<SpawnObjects>();
        coinPrefab = spawnObjects.coinPrefab;
        ghostPrefab = spawnObjects.ghostPrefab;

        objects = new[]
        {
            // beatNum, prefab, trackNum
            new SpawnableObject(0, coinPrefab, 1),
            new SpawnableObject(2, coinPrefab, 1),
            new SpawnableObject(2, coinPrefab, 1),
            new SpawnableObject(2, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 0),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 2),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 0),
            new SpawnableObject(1, coinPrefab, 1),
            new SpawnableObject(1, coinPrefab, 2),
            new SpawnableObject(0.5f, coinPrefab, 1),
            new SpawnableObject(0.5f, coinPrefab, 0),
            new SpawnableObject(0.5f, coinPrefab, 1),
            new SpawnableObject(0.5f, coinPrefab, 2),
            new SpawnableObject(0.5f, coinPrefab, 1),
            new SpawnableObject(0.5f, coinPrefab, 0),
            new SpawnableObject(0.5f, coinPrefab, 1),
            new SpawnableObject(0.5f, coinPrefab, 2),
            new SpawnableObject(1f, ghostPrefab, 1),
            new SpawnableObject(1f, ghostPrefab, 0),
            new SpawnableObject(0.5f, coinPrefab, 1),
            new SpawnableObject(0.5f, coinPrefab, 0),
            new SpawnableObject(1f, ghostPrefab, 2)
        };
    }
}