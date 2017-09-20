using UnityEngine;
using EliotScripts.ObjectPool;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
//using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector]
    public bool[] diamonds;

    [HideInInspector]
    public ObjectPool pool;

    [HideInInspector]
    public int coins;

    public int scrollSpeed;
    public RhythmTool rhythmTool;

    [SerializeField]
    private GameObject statsPanel;

    [SerializeField]
    private Button continueButton;

    [SerializeField]
    private string restartButton = "Restart";

    [SerializeField]
    private Text songText;

    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private string playerPrefsHighScore = "HighScore";

    [SerializeField]
    private GameObject highScoreText;

    [SerializeField]
    private Text coinText;

    [SerializeField]
    private GameObject blueDiamondStat;

    [SerializeField]
    private GameObject greenDiamondStat;

    [SerializeField]
    private GameObject orangeDiamondStat;

    private int currentObj;

    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("ERROR: GameManager already exists");
        }
        else
        {
            instance = this;
        }

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
            case PersistentManager.Scenes.Persistent:
                break;

            case PersistentManager.Scenes.Test:
                songText.text = "Checkpoint - Nitro Fun, Hyper Potions";
                break;

            case PersistentManager.Scenes.Level1_2:
                songText.text = "Cyberpunk Moonlight Sonata - Joth";
                break;
   
            default:
                break;
        }

        diamonds = new bool[3];
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

    public void ShowStats()
    {
        statsPanel.SetActive(true);
        continueButton.Select();
        playerMovement.canMove = false;
        coinText.text = coins.ToString();

        if (coins > PersistentManager.Instance.GetHighScore(PersistentManager.Instance.GetLevel()))
        {
            PersistentManager.Instance.SetHighScore(PersistentManager.Instance.GetLevel(), coins);
            highScoreText.SetActive(true);
        }

        if (diamonds[0] == true)
        {
            blueDiamondStat.SetActive(true);
        }

        if (diamonds[1] == true)
        {
            greenDiamondStat.SetActive(true);
        }

        if (diamonds[2] == true)
        {
            orangeDiamondStat.SetActive(true);
        }

        PersistentManager.Instance.SetDiamonds(PersistentManager.Instance.GetLevel(), diamonds);
    }

    public void ReturnToLevelSelection()
    {
        PersistentManager.Instance.LoadScene(PersistentManager.Scenes.Title, PersistentManager.Instance.CurrentScene);
    }
}
