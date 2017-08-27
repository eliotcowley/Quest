using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance;
    public Text loadingText;

    [HideInInspector]
    public Scenes CurrentScene = startScene;

    [SerializeField]
    private const Scenes startScene = Scenes.TurnOnSound;

    [SerializeField]
    private AudioController audioController;

    [SerializeField]
    private GameObject persistentCamera;

    [SerializeField]
    private string[] sceneNames;

    [SerializeField]
    private SpriteRenderer fader;

    [SerializeField]
    private float smooth = 0.5f;

    [SerializeField]
    private float fadeTime = 1f;

    private bool fadeOut = false;
    private bool fadeIn = false;
    private Color zeroAlpha = new Color(0f, 0f, 0f, 0f);
    private Color fullAlpha = new Color(0f, 0f, 0f, 1f);
    private Scenes sceneToUnload;
    private Scenes sceneToLoad;

    public enum Scenes
    {
        Persistent = 0,
        TurnOnSound = 1,
        Title = 2,
        Test = 3,
        LevelSelect = 4,
        Level1_2 = 5
    }

    private void Start()
    {
        LoadScene(startScene);

        if (Instance != null)
        {
            Debug.LogError("ERROR: More than one PersistentManager in scene");
        }
        else
        {
            Instance = this;
        }

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void Update()
    {
        if (fadeOut && (fader.color.a < 1f))
        {
            fader.color = Color.Lerp(fader.color, fullAlpha, smooth * Time.deltaTime);
        }
        else if (fadeIn && (fader.color.a > 0f))
        {
            fader.color = Color.Lerp(fader.color, zeroAlpha, smooth * Time.deltaTime);
        }
    }

    public void LoadScene(Scenes sceneToLoad, Scenes sceneToUnload)
    {
        this.sceneToLoad = sceneToLoad;
        this.sceneToUnload = sceneToUnload;
        FadeOut();
        Invoke("UnloadScene", fadeTime);
    }

    public void LoadScene(Scenes sceneToLoad)
    {
        SceneManager.LoadSceneAsync(sceneNames[(int)sceneToLoad], LoadSceneMode.Additive);

        if (!((audioController.CurrentSong == AudioController.BGM.Title) && ((sceneToLoad == Scenes.Title) || (sceneToLoad == Scenes.LevelSelect))))
        {
            audioController.Stop();
        }

        CurrentScene = sceneToLoad;

        switch (sceneToLoad)
        {
            case Scenes.Title:
                audioController.Play(AudioController.BGM.Title);
                break;

            case Scenes.LevelSelect:
                if ((audioController.CurrentSong != AudioController.BGM.Title) ||
                    ((audioController.CurrentSong == AudioController.BGM.Title) && (!audioController.audioSource.isPlaying)))
                {
                    audioController.Play(AudioController.BGM.Title);
                }
                break;
        }
    }

    public int GetLevel()
    {
        switch (CurrentScene)
        {
            case Scenes.Test:
                return 0;
       
            case Scenes.Level1_2:
                return 1;

            default:
                return -1;
        }
    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        persistentCamera.SetActive(true);
        loadingText.gameObject.SetActive(true);
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name != sceneNames[(int)Scenes.Persistent])
        {
            persistentCamera.SetActive(false);
            StartCoroutine(SetActiveScene(scene));
            loadingText.gameObject.SetActive(false);
        }
    }

    private IEnumerator SetActiveScene(Scene scene)
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.SetActiveScene(scene);
        FadeIn();
    }

    private void FadeOut()
    {
        fader.color = zeroAlpha;
        fadeOut = true;
        fadeIn = false;
    }

    private void FadeIn()
    {
        fadeOut = false;
        fadeIn = true;
    }

    private void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(sceneNames[(int)sceneToUnload]);
        LoadScene(sceneToLoad);
    }
}
