using UnityEngine;
using System.Collections;
using EliotScripts.ObjectPool;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public ObjectPool pool;

    [HideInInspector]
    public int coins;

    public int scrollSpeed;

    private int currentObj;

    void Start()
    {
        if (pool != null)
        {
            Debug.LogError("ERROR: Object pool already exists");
            return;
        }
        pool = GetComponent<ObjectPool>();
        coins = 0;
        Random.seed = 42;   // So long, and thanks for all the fish
    }
}
