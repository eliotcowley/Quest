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

    private void Start()
    {
        buttonToStartSelected.Select();
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
