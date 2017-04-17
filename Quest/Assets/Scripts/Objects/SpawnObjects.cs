﻿using UnityEngine;
using System.Collections;
using EliotScripts.ObjectPool;

public class SpawnObjects : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject ghostPrefab;
    public GameObject heartPrefab;
    public GameObject starPrefab;
    public GameObject blueDiamondPrefab;
    public GameObject greenDiamondPrefab;
    public GameObject orangeDiamondPrefab;
    public GameObject barrierPrefab;
    public GameObject goalPrefab;

    [SerializeField]
    private Transform[] positions;
    
    [SerializeField]
    private ObjectPool pool;

    [SerializeField]
    private int startBeat = 88;

    private int nextPosition;
    private int index;
    private GameObject nextPrefab;
    private int nextSample;
    private float currentSample;
    private int currentBeat = 0;
    private int nextBeat;

    private void Start()
    {
        index = 0;
        nextBeat = startBeat;
    }

    public void Spawn()
    {
        currentBeat++;

        if (!SpawnableObjectList.prefabsSet || (index >= SpawnableObjectList.objects.Length) || (currentBeat < nextBeat))
        {
            return;
        }

        GameObject prefab;
        nextPosition = SpawnableObjectList.objects[index].trackNum;
        nextPrefab = SpawnableObjectList.objects[index++].go;

        if (nextPrefab == null)
        {
            Debug.LogError("No prefab!");
        }

        GameObject obj;

        switch(nextPrefab.tag)
        {
            case "Coin":
                prefab = pool.PullFromPool("Coin");
                obj = coinPrefab;
                break;

            case "Enemy":
                prefab = pool.PullFromPool("Enemy");
                obj = ghostPrefab;
                break;

            case "Heart":
                prefab = pool.PullFromPool("Heart");
                obj = heartPrefab;
                break;

            case "Star":
                prefab = pool.PullFromPool("Star");
                obj = starPrefab;
                break;

            case "BlueDiamond":
                prefab = pool.PullFromPool("BlueDiamond");
                obj = blueDiamondPrefab;
                break;

            case "GreenDiamond":
                prefab = pool.PullFromPool("GreenDiamond");
                obj = greenDiamondPrefab;
                break;

            case "OrangeDiamond":
                prefab = pool.PullFromPool("OrangeDiamond");
                obj = orangeDiamondPrefab;
                break;

            case "Barrier":
                prefab = pool.PullFromPool("Barrier");
                obj = barrierPrefab;
                break;

            case "Goal":
                prefab = pool.PullFromPool("Goal");
                obj = goalPrefab;
                break;

            default:
                prefab = pool.PullFromPool("Coin");
                obj = coinPrefab;
                break; 
        }

        if (prefab == null)
        {
            Instantiate(obj, positions[nextPosition].position, Quaternion.identity);
        }
        else
        {
            prefab.transform.position = positions[nextPosition].position;
        }

        if (index < SpawnableObjectList.objects.Length)
        {
            nextBeat = SpawnableObjectList.objects[index].beatNum;
        }
        
        currentBeat = 0;
    }
}
