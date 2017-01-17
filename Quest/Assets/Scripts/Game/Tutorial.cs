using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject moveText;
    public GameObject swordText;
    public GameObject coinText;
    public RhythmTool rhythmTool;

    public const int showTextBeat = 18;
    public const int showSwordTextBeat = 50;
    public const int showCoinTextBeat = 82;
    public const int textDisplayDuration = 16;

    private int currentBeat = 0;

    private void Start()
    {
        moveText.GetComponent<Text>().text = "Use " + (InputManager.IsGamepadConnected() ? "left stick " : "arrow keys ") + "to switch lanes";
        swordText.GetComponent<Text>().text = "Press " + (InputManager.IsGamepadConnected() ? "A " : "left ctrl ") + "to swing sword";
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
