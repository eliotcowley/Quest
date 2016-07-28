using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
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

    private List<GameObject> hearts;

    [HideInInspector]
    public int currentHealth;

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
        Debug.Log(string.Join(",", hearts.Select(h => h.name).ToArray()));
        UpdateHearts();
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
            currentHealth += amount;
            UpdateHearts();
            if (currentHealth <= 0)
            {
                player.SetActive(false);
                gameOver.SetActive(true);
            }
        }
    }
}
