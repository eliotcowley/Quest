using UnityEngine;
using EliotScripts.ObjectPool;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public ObjectPool pool;

    [HideInInspector]
    public int coins;

    [HideInInspector]
    public int diamonds;

    public int scrollSpeed;
    public RhythmTool rhythmTool;

    [SerializeField]
    private string restartButton = "Restart";

    [SerializeField]
    private Text songText;

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
        //Random.InitState(42); // So long, and thanks for all the fish
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        Debug.Log("Joysticks connected: " + InputManager.IsGamepadConnected());

        switch (PersistentManager.Instance.CurrentScene)
        {
            case PersistentManager.Scenes.Title:
                break;
            case PersistentManager.Scenes.Test:
                songText.text = "Checkpoint - Nitro Fun, Hyper Potions";
                break;
            case PersistentManager.Scenes.Persistent:
                break;
            default:
                break;
        }
    }

    void Update()
    {
        // Restart the level
        if (Input.GetButtonDown(restartButton))
        {
            Restart();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        Pause.paused = false;
        rhythmTool.Stop();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PersistentManager.Instance.LoadScene(PersistentManager.Instance.CurrentScene, PersistentManager.Instance.CurrentScene);
    }
}
