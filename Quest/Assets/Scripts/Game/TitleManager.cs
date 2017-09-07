using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using EliotScripts.ObjectPool;

public class TitleManager : MonoBehaviour
{
    public int ScrollSpeed = 140;

    [HideInInspector]
    public ObjectPool Pool;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private GameObject titleMenu;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private GameObject levelSelectionMenu;

    [SerializeField]
    private Button levelSelectionButtonToStartSelected;

    [SerializeField]
    private Text fullscreenButtonText;

    [SerializeField]
    private Text resolutionText;

    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private AudioMixer musicMixer;

    [SerializeField]
    private AudioMixer sfxMixer;

    [SerializeField]
    private Button fullScreenButton;

    [SerializeField]
    private Animator titleTextAnimator;

    [SerializeField]
    private float fadeTime = 0.5f;

    [SerializeField]
    private Text coinCountText;

    [SerializeField]
    private GameObject diamondPanel;

    private bool fullscreen;
    private Resolution[] resolutions;
    private Resolution currentResolution;
    private int resolutionIndex = 0;
    private Button[] mainMenuButtons;
    private Button[] levelSelectionButtons;
    private Selectable[] optionsSelectables;
    private Animator[] mainMenuButtonAnimators;
    private Animator[] levelSelectionAnimators;
    private Animator[] optionsAnimators;
    private int selectedLevel;
    private List<GameObject> diamondImages;

    private void Start()
    {
        startButton.Select();
        fullscreen = Screen.fullScreen;
        resolutions = Screen.resolutions;
        currentResolution = Screen.currentResolution;

        // Get current resolution
        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((resolutions[i].width == currentResolution.width) && (resolutions[i].height == currentResolution.height))
            {
                resolutionIndex = i;
                break;
            }
        }

        fullscreenButtonText.text = fullscreen ? "On" : "Off";
        resolutionText.text = currentResolution.width + " x " + currentResolution.height;
        musicSlider.value = GetMusicVolume();
        Pool = GetComponent<ObjectPool>();
        mainMenuButtons = titleMenu.GetComponentsInChildren<Button>();
        mainMenuButtonAnimators = titleMenu.GetComponentsInChildren<Animator>();
        levelSelectionAnimators = levelSelectionMenu.GetComponentsInChildren<Animator>();
        levelSelectionButtons = levelSelectionMenu.GetComponentsInChildren<Button>();
        optionsSelectables = optionsMenu.GetComponentsInChildren<Selectable>();
        optionsAnimators = optionsMenu.GetComponentsInChildren<Animator>();

        Transform[] diamondTransforms = diamondPanel.GetComponentsInChildren<Transform>();
        diamondImages = new List<GameObject>();
        foreach (Transform child in diamondTransforms)
        {
            diamondImages.Add(child.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

            #else
            Application.Quit();

            #endif
        }
    }

    public void ToggleOptions()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
        titleMenu.SetActive(!titleMenu.activeSelf);

        if (optionsMenu.activeSelf)
        {
            fullScreenButton.Select();
        }
        else
        {
            startButton.Select();
        }
    }

    public void ToggleFullscreen()
    {
        fullscreen = !fullscreen;
        fullscreenButtonText.text = fullscreen ? "On" : "Off";
    }

    public void IterateResolution()
    {
        if ((resolutionIndex + 1) >= resolutions.Length)
        {
            resolutionIndex = 0;
        }
        else
        {
            resolutionIndex++;
        }

        currentResolution = resolutions[resolutionIndex];
        resolutionText.text = currentResolution.width + " x " + currentResolution.height;
    }

    public void UpdateMusicVolume()
    {
        musicMixer.SetFloat("MasterVolume", musicSlider.value);
    }

    public void UpdateSFXVolume()
    {
        sfxMixer.SetFloat("MasterVolume", sfxSlider.value);
    }

    public void SetMusicVolume(float value)
    {
        musicMixer.SetFloat("MasterVolume", value);
    }

    public void SetSfxVolume(float value)
    {
        sfxMixer.SetFloat("MasterVolume", value);
    }

    public float GetMusicVolume()
    {
        float value;
        musicMixer.GetFloat("MasterVolume", out value);
        return value;
    }

    public float GetSfxVolume()
    {
        float value;
        sfxMixer.GetFloat("MasterVolume", out value);
        return value;
    }

    public void Apply()
    {
        Screen.SetResolution(currentResolution.width, currentResolution.height, fullscreen);
    }

    public void GoToLevelScene()
    {
        StartCoroutine(OpenLevelSelection());
    }

    public void GoHomeFromLevelSelection()
    {
        StartCoroutine(OpenHomeFromLevelSelection());
    }

    public void GoToOptions()
    {
        StartCoroutine(OpenOptions());
    }

    public void GoHomeFromOptions()
    {
        StartCoroutine(OpenHomeFromOptions());
    }

    private IEnumerator OpenLevelSelection()
    {
        foreach (Button button in mainMenuButtons)
        {
            button.interactable = false;
        }

        // Fade out main menu buttons
        foreach (Animator animator in mainMenuButtonAnimators)
        {
            animator.SetTrigger(Constants.FadeOutAnimatorTrigger);
        }

        titleTextAnimator.SetTrigger(Constants.FadeOutAnimatorTrigger);
        yield return new WaitForSeconds(fadeTime);
        titleMenu.SetActive(false);
        levelSelectionMenu.SetActive(true);

        foreach (Animator animator in levelSelectionAnimators)
        {
            animator.SetTrigger(Constants.FadeInAnimatorTrigger);
        }

        foreach (Button button in levelSelectionButtons)
        {
            button.interactable = true;
        }

        levelSelectionButtonToStartSelected.Select();
    }

    private IEnumerator OpenHomeFromLevelSelection()
    {
        foreach (Button button in levelSelectionButtons)
        {
            button.interactable = false;
        }

        foreach (Animator animator in levelSelectionAnimators)
        {
            animator.SetTrigger(Constants.FadeOutAnimatorTrigger);
        }

        titleTextAnimator.SetTrigger(Constants.FadeInAnimatorTrigger);
        yield return new WaitForSeconds(fadeTime);
        titleMenu.SetActive(true);
        levelSelectionMenu.SetActive(false);

        foreach (Animator animator in mainMenuButtonAnimators)
        {
            animator.SetTrigger(Constants.FadeInAnimatorTrigger);
        }

        foreach (Button button in mainMenuButtons)
        {
            button.interactable = true;
        }

        startButton.Select();
    }

    private IEnumerator OpenOptions()
    {
        foreach (Button button in mainMenuButtons)
        {
            button.interactable = false;
        }

        foreach (Animator animator in mainMenuButtonAnimators)
        {
            animator.SetTrigger(Constants.FadeOutAnimatorTrigger);
        }

        yield return new WaitForSeconds(fadeTime);
        optionsMenu.SetActive(true);
        titleMenu.SetActive(false);

        foreach (Animator animator in optionsAnimators)
        {
            animator.SetTrigger(Constants.FadeInAnimatorTrigger);
        }

        foreach (Selectable selectable in optionsSelectables)
        {
            selectable.interactable = true;
        }

        fullScreenButton.Select();
    }

    private IEnumerator OpenHomeFromOptions()
    {
        foreach (Selectable selectable in optionsSelectables)
        {
            selectable.interactable = false;
        }

        foreach (Animator animator in optionsAnimators)
        {
            animator.SetTrigger(Constants.FadeOutAnimatorTrigger);
        }

        yield return new WaitForSeconds(fadeTime);
        titleMenu.SetActive(true);
        optionsMenu.SetActive(false);

        foreach (Animator animator in mainMenuButtonAnimators)
        {
            animator.SetTrigger(Constants.FadeInAnimatorTrigger);
        }

        foreach (Button button in mainMenuButtons)
        {
            button.interactable = true;
        }

        startButton.Select();
    }

    public void LoadLevel(int level)
    {
        switch (level)
        {
            case 1:
                PersistentManager.Instance.LoadScene(PersistentManager.Scenes.Test, PersistentManager.Instance.CurrentScene);
                break;

            case 2:
                PersistentManager.Instance.LoadScene(PersistentManager.Scenes.Level1_2, PersistentManager.Instance.CurrentScene);
                break;

            default:
                break;
        }
    }

    public void SetSelectedLevel(int level)
    {
        selectedLevel = level;
        coinCountText.text = PersistentManager.Instance.GetHighScore(level-1).ToString();
        bool[] diamonds = PersistentManager.Instance.GetDiamonds(level - 1);

        for (int i = 1; i < diamondImages.Count; i++)
        {
            if (diamonds[i-1])
            {
                diamondImages[i].SetActive(true);
            }
            else
            {
                diamondImages[i].SetActive(false);
            }
        }
    }
}
