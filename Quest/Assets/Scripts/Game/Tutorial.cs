using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject moveText;
    public GameObject swordText;
    public GameObject switchText;
    public GameObject coinText;
    public RhythmTool rhythmTool;

    public const int showTextBeat = 10;
    public const int showSwordTextBeat = 34;
    public const int showSwitchTextBeat = 58;
    public const int showCoinTextBeat = 82;
    public const int textDisplayDuration = 16;

    private int currentBeat = 0;

    private void Start()
    {
        moveText.GetComponent<Text>().text = "Use " + (InputManager.IsGamepadConnected() ? "left stick " : "arrow keys ") + "to switch lanes";
        swordText.GetComponent<Text>().text = "Press " + (InputManager.IsGamepadConnected() ? "A " : "left ctrl ") + "to attack";
        switchText.GetComponent<Text>().text = "Press " + (InputManager.IsGamepadConnected() ? "Y " : "left shift ") + "to switch the leader";
    }

    public void OnBeat(Beat beat)
    {
        switch (currentBeat)
        {
            case showTextBeat:
                moveText.SetActive(true);
                break;

            case (showTextBeat + textDisplayDuration):
                moveText.SetActive(false);
                break;

            case showSwordTextBeat:
                swordText.SetActive(true);
                break;

            case (showSwordTextBeat + textDisplayDuration):
                swordText.SetActive(false);
                break;

            case showSwitchTextBeat:
                switchText.SetActive(true);
                break;

            case (showSwitchTextBeat + textDisplayDuration):
                switchText.SetActive(false);
                break;

            case showCoinTextBeat:
                coinText.SetActive(true);
                break;

            case (showCoinTextBeat + textDisplayDuration):
                coinText.SetActive(false);
                break;
        }

        currentBeat++;
    }
}
