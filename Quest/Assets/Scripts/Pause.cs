using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField]
    private string pauseButtonString = "Pause";

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private AudioSource musicAudioSource;

    [SerializeField]
    private Button resumeButton;

    [SerializeField]
    private Button pauseButton;

    public static bool paused = false;

    private void Update()
    {
        if (Input.GetButtonDown(pauseButtonString))
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void Resume()
    {
        pauseScreen.SetActive(false);
        paused = false;
        Time.timeScale = 1f;
        musicAudioSource.UnPause();
        pauseButton.interactable = true;
    }

    public void PauseGame()
    {
        if (!paused)
        {
            pauseScreen.SetActive(true);
            paused = true;
            Time.timeScale = 0f;
            musicAudioSource.Pause();
            resumeButton.Select();
            pauseButton.interactable = false;
        }
        
    }
}
