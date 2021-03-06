﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector]
    public int currentHealth;

    [HideInInspector]
    public static bool canRestart = false;

    [SerializeField]
    private int maxHealth = 3;

    [SerializeField]
    private GameObject heartPrefab;

    [SerializeField]
    private Transform heartStartingPosition;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject gameOver;

    [SerializeField]
    private Text restartText;

    [SerializeField]
    private string controllerAButton = "Fire1Controller";

    private List<GameObject> hearts;
    private SFXMixer sfxMixer;

    // Use this for initialization
    void Start ()
    {
        currentHealth = maxHealth;
        hearts = new List<GameObject>();
        List<Transform> transforms = heartStartingPosition.GetComponentsInChildren<Transform>().ToList();
  
        hearts = transforms.Select(t => t.gameObject)
            .Where(t => t.GetInstanceID() != heartStartingPosition.GetInstanceID())
            .ToList();

        hearts.Sort((x, y) => string.Compare(x.name, y.name));
        //Debug.Log(string.Join(",", hearts.Select(h => h.name).ToArray()));
        UpdateHearts();
        //Debug.Log("Number of controllers detected: " + Input.GetJoystickNames().Length);
        sfxMixer = SFXMixer.instance;
    }

    void Update()
    {
        if (canRestart)
        {
#if UNITY_ANDROID

#endif
            // Xbox controller connected
            if (InputManager.IsGamepadConnected())
            {
                // Restart
                if (Input.GetButtonDown(controllerAButton))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    canRestart = false;
                }
            }
        }
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < currentHealth; i++)
        {
            hearts[i].SetActive(true);
        }
        for (int i = currentHealth; i < maxHealth; i++)
        {
            hearts[i].SetActive(false);
        }
    }

    public void ChangeHealth(int amount)
    {
        if (player.activeSelf)
        {
            if (amount > 0)
            {
                if (currentHealth >= maxHealth)
                {
                    return;
                }
            }
            currentHealth += amount;
            UpdateHearts();
            if (currentHealth <= 0)
            {
                // Game over
                sfxMixer.PlaySound(SFXMixer.Sounds.PlayerDie);
#if UNITY_ANDROID
                restartText.text = "Tap to restart";
#endif

                if (InputManager.IsGamepadConnected())
                {
                    restartText.text = "Press A to restart";
                }

                gameOver.SetActive(true);
                canRestart = true;
                player.SetActive(false);
            }
        }
    }
}
