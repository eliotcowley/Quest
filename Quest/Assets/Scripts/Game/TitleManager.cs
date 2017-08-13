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

    private bool fullscreen;
    private Resolution[] resolutions;
    private Resolution currentResolution;
    private int resolutionIndex = 0;
    private Button[] mainMenuButtons;
    private Button[] levelSelectionButtons;
    private Animator[] mainMenuButtonAnimators;
    private Animator[] levelSelectionAnimators;

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
        yield return new WaitForSeconds(0.5f);
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
        yield return new WaitForSeconds(0.5f);
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
    }
}
