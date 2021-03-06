﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{
    [SerializeField]
    private string pauseButtonString = "Pause";

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private Button resumeButton;

    [SerializeField]
    private Button pauseButton;

    [SerializeField]
    private PlayerMovement playerMovement;

    public static bool paused = false;
    public RhythmTool rhythmTool;

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
                if (playerMovement.canMove)
                {
                    PauseGame();
                }
            }
        }
    }

    public void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);
        pauseScreen.SetActive(false);
        paused = false;
        rhythmTool.UnPause();
        Time.timeScale = 1f;
        pauseButton.interactable = true;
    }

    public void PauseGame()
    {
        if (!paused)
        {
            pauseScreen.SetActive(true);
            paused = true;
            resumeButton.Select();
            rhythmTool.Pause();
            Time.timeScale = 0f;
            pauseButton.interactable = false;
        }
        
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void DeselectOtherButtons()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ReturnToTitleScreen()
    {
        Time.timeScale = 1f;
        PersistentManager.Instance.LoadScene(PersistentManager.Scenes.Title, PersistentManager.Instance.CurrentScene);
    }
}
