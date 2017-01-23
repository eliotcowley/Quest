﻿using UnityEngine;
using System.Collections;

public class SpawnableObjectList : MonoBehaviour
{
    private SpawnObjects spawnObjects;

    public static GameObject coinPrefab;
    public static GameObject ghostPrefab;
    public static SpawnableObject[] objects;
    public static GameObject heartPrefab;
    public static GameObject starPrefab;
    public static GameObject blueDiamondPrefab;
    public static GameObject greenDiamondPrefab;
    public static GameObject orangeDiamondPrefab;

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
            new SpawnableObject(2, heartPrefab, 1),
            new SpawnableObject(2, starPrefab, 1),
            new SpawnableObject(2, ghostPrefab, 1),
            new SpawnableObject(1, ghostPrefab, 0),
            new SpawnableObject(2, blueDiamondPrefab, 1),
            new SpawnableObject(2, greenDiamondPrefab, 2),
            new SpawnableObject(2, orangeDiamondPrefab, 0)
        };
    }
}