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

    private void Start()
    {
        startButton.Select();
    }

    public void ToggleOptions()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
        titleMenu.SetActive(!titleMenu.activeSelf);
    }
}
