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

    [SerializeField]
    private GameObject blueDiamond;

    [SerializeField]
    private GameObject greenDiamond;

    [SerializeField]
    private GameObject orangeDiamond;

    private Button[] buttons;

    private void Start()
    {
        buttonToStartSelected.Select();
        buttons = canvas.GetComponentsInChildren<Button>();
        coinCountText.text = PlayerPrefs.GetInt(playerPrefsHighScore, 0).ToString();

        if (PlayerPrefs.GetInt("Blue", 0) == 1)
        {
            blueDiamond.SetActive(true);
        }

        if (PlayerPrefs.GetInt("Green", 0) == 1)
        {
            greenDiamond.SetActive(true);
        }

        if (PlayerPrefs.GetInt("Orange", 0) == 1)
        {
            orangeDiamond.SetActive(true);
        }
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

            case 2:
                PersistentManager.Instance.LoadScene(PersistentManager.Scenes.Level1_2, PersistentManager.Instance.CurrentScene);
                break;

            default:
                break;
        }
    }
}
