using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Button buttonToStartSelected;

    [SerializeField]
    private string backButtonName = "Cancel";

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private Text coinCountText;

    [SerializeField]
    private string playerPrefsHighScore = "HighScore";

    private Button[] buttons;

    private void Start()
    {
        buttonToStartSelected.Select();
        buttons = canvas.GetComponentsInChildren<Button>();
        coinCountText.text = PlayerPrefs.GetInt(playerPrefsHighScore, 0).ToString();
    }

    private void Update()
    {
        if (Input.GetButtonDown(backButtonName))
        {
            GoBackToTitleScene();
        }
    }

    public void GoBackToTitleScene()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        PersistentManager.Instance.LoadScene(PersistentManager.Scenes.Title, PersistentManager.Instance.CurrentScene);
    }

    public void LoadLevel(int level)
    {
        switch (level)
        {
            case 1:
                PersistentManager.Instance.LoadScene(PersistentManager.Scenes.Test, PersistentManager.Instance.CurrentScene);
                break;

            default:
                break;
        }
    }
}
