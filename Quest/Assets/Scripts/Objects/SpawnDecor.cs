using UnityEngine;
using System.Collections;
using EliotScripts.ObjectPool;

public class SpawnDecor : MonoBehaviour
{
    [SerializeField]
    private Transform[] positions;

    [SerializeField]
    private float maxTime;

    public GameObject[] decorPrefabs;

    [SerializeField]
    private ObjectPool pool;

    private float nextTime;
    private int nextPosition;

    // Use this for initialization
    void Start()
    {
        InitSpawn();
    }

    private void Spawn()
    {
        GameObject decor = pool.PullFromPool("Decor");

        if (decor == null)
        {
            Instantiate(decorPrefabs[Random.Range(0, decorPrefabs.Length)], positions[nextPosition].position, Quaternion.identity);
        }
        else
        {
            decor.transform.position = positions[nextPosition].position;
        }

        InitSpawn();  
    }

    private void InitSpawn()
    {
        nextTime = Random.Range(1f, maxTime);
        nextPosition = Random.Range(0, positions.Length);
        Invoke("Spawn", nextTime);
    }
}
