using UnityEngine;
using System.Collections;

public class SpawnableObjectList : MonoBehaviour
{
    private SpawnObjects spawnObjects;

    public static GameObject coinPrefab;
    public static GameObject ghostPrefab;
    public static ArrayList objects;
    public static GameObject heartPrefab;
    public static GameObject starPrefab;
    public static GameObject blueDiamondPrefab;
    public static GameObject greenDiamondPrefab;
    public static GameObject orangeDiamondPrefab;
    public static GameObject barrierPrefab;
    public static GameObject goalPrefab;
    public static GameObject snailPrefab;
    public static GameObject mushroomPrefab;

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
        starPrefab = spawnObjects.starPrefab;
        blueDiamondPrefab = spawnObjects.blueDiamondPrefab;
        greenDiamondPrefab = spawnObjects.greenDiamondPrefab;
        orangeDiamondPrefab = spawnObjects.orangeDiamondPrefab;
        barrierPrefab = spawnObjects.barrierPrefab;
        goalPrefab = spawnObjects.goalPrefab;
        snailPrefab = spawnObjects.snailPrefab;
        mushroomPrefab = spawnObjects.mushroomPrefab;

        objects = new ArrayList
        {
            // Level 0
            new SpawnableObject[]
            {
                // beatNum, prefab, trackNum
                new SpawnableObject(0, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(2, coinPrefab, 1),
                new SpawnableObject(1, snailPrefab, 3),
                new SpawnableObject(1, coinPrefab, 0),
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
                new SpawnableObject(1, snailPrefab, 4),
                new SpawnableObject(1, ghostPrefab, 1),
                new SpawnableObject(4, ghostPrefab, 0),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 0),
                new SpawnableObject(2, ghostPrefab, 2),
                new SpawnableObject(2, heartPrefab, 1),
                new SpawnableObject(2, starPrefab, 1),
                new SpawnableObject(2, ghostPrefab, 1),
                new SpawnableObject(1, ghostPrefab, 0),
                new SpawnableObject(2, blueDiamondPrefab, 1),
                new SpawnableObject(2, greenDiamondPrefab, 2),
                new SpawnableObject(2, orangeDiamondPrefab, 0),
                new SpawnableObject(2, barrierPrefab, 1),
                new SpawnableObject(4, goalPrefab, 1)
            },

            // Level 1
            new SpawnableObject[]
            {
                // beatNum, prefab, trackNum
                new SpawnableObject(0, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1),
                new SpawnableObject(1, coinPrefab, 1)
            }
        };

        spawnObjects.SetupList();
    }
}