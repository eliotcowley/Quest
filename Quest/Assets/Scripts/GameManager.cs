using UnityEngine;
using EliotScripts.ObjectPool;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public ObjectPool pool;

    [HideInInspector]
    public int coins;

    public int scrollSpeed;

    [SerializeField]
    private string restartButton = "Restart";

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

    void Update()
    {
        // Restart the level
        if (Input.GetButtonDown(restartButton))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
