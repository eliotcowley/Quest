using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

    [SerializeField]
    private GameObject titleMenu;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private Text fullscreenButtonText;

    [SerializeField]
    private Text resolutionText;

    private bool fullscreen;
    private Resolution[] resolutions;
    private Resolution currentResolution;
    private int resolutionIndex = 0;

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
    }

    public void ToggleOptions()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
        titleMenu.SetActive(!titleMenu.activeSelf);
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

    public void Apply()
    {
        Screen.SetResolution(currentResolution.width, currentResolution.height, fullscreen);
    }
}
