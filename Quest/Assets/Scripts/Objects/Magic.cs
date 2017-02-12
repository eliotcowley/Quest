using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    [SerializeField]
    private string barrierTag = "Barrier";

    private GameManager gm;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.tag == enemyTag)
        //{
        //    collision.gameObject.GetComponent<EnemyDie>().Die();
        //}

        if (collision.gameObject.tag == barrierTag)
        {
            gm.pool.AddToPool(collision.gameObject);
        }
    }
}
