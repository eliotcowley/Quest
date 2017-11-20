using UnityEngine;
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
    public GameObject snailPrefab;
    public GameObject mushroomPrefab;

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
    private SpawnableObject[] objectList;

    private void Start()
    {
        index = 0;
        nextBeat = startBeat;
    }

    public void Spawn()
    {
        Debug.Log("Spawn");
        currentBeat++;

        if (!SpawnableObjectList.prefabsSet
            || (objectList == null)
            || (index >= objectList.Length) 
            || (currentBeat < nextBeat))
        {
            return;
        }

        GameObject prefab;
        nextPosition = objectList[index].trackNum;
        nextPrefab = objectList[index++].go;

        if (nextPrefab == null)
        {
            Debug.LogError("No prefab!");
        }

        GameObject obj;

        switch(nextPrefab.tag)
        {
            case "Coin":
                prefab = pool.PullFromPool(Constants.CoinTag);
                obj = coinPrefab;
                break;

            case "Enemy":
                prefab = pool.PullFromPool(Constants.EnemyTag);
                obj = ghostPrefab;
                break;

            case "Heart":
                prefab = pool.PullFromPool(Constants.HeartTag);
                obj = heartPrefab;
                break;

            case "Star":
                prefab = pool.PullFromPool(Constants.StarTag);
                obj = starPrefab;
                break;

            case "BlueDiamond":
                prefab = pool.PullFromPool(Constants.BlueDiamondTag);
                obj = blueDiamondPrefab;
                break;

            case "GreenDiamond":
                prefab = pool.PullFromPool(Constants.GreenDiamondTag);
                obj = greenDiamondPrefab;
                break;

            case "OrangeDiamond":
                prefab = pool.PullFromPool(Constants.OrangeDiamondTag);
                obj = orangeDiamondPrefab;
                break;

            case "Barrier":
                prefab = pool.PullFromPool(Constants.BarrierTag);
                obj = barrierPrefab;
                break;

            case "Goal":
                prefab = pool.PullFromPool(Constants.GoalTag);
                obj = goalPrefab;
                break;

            case "Snail":
                prefab = pool.PullFromPool(Constants.SnailTag);
                obj = snailPrefab;
                break;

            case "Mushroom":
                prefab = pool.PullFromPool(Constants.MushroomTag);
                obj = mushroomPrefab;
                break;

            default:
                prefab = pool.PullFromPool(Constants.CoinTag);
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

        if (index < objectList.Length)
        {
            nextBeat = objectList[index].beatNum;
        }
        
        currentBeat = 0;
    }

    public void SetupList()
    {
        int level = PersistentManager.Instance.GetLevel();
        Debug.Log("Level " + level);

        if (level == -1)
        {
            Debug.LogError("ERROR: No object list found for this level");
        }
        else
        {
            objectList = (SpawnableObject[])SpawnableObjectList.objects[level];
        }
    }
}
