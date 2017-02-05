using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    [SerializeField]
    private string enemyTag = "Enemy";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == enemyTag)
        {
            collision.gameObject.GetComponent<EnemyDie>().Die();
        }
    }
}
