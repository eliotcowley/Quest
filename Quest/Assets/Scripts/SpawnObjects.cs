using UnityEngine;
using System.Collections;
using EliotScripts.ObjectPool;

public class SpawnObjects : MonoBehaviour
{
    [SerializeField]
    private Transform[] positions;
    
    [SerializeField]
    private ObjectPool pool;

    [SerializeField]
    private float startTime = 8.5f;

    [SerializeField]
    private float beatLength;

    private float nextTime;
    private int nextPosition;
    private int index;
    private GameObject nextPrefab;

    public GameObject coinPrefab;
    public GameObject ghostPrefab;
    public GameObject heartPrefab;

    private void Start()
    {
        nextTime = startTime;
        index = 0;
        InitSpawn();
    }

    private void Spawn()
    {
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
            nextTime = SpawnableObjectList.objects[index].beatNum * beatLength;
            Invoke("Spawn", nextTime);
        }
    }

    private void InitSpawn()
    {
        Invoke("Spawn", nextTime);
    }
}
