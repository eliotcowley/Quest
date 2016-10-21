using UnityEngine;
using System.Collections;

public class EnemyPasses : MonoBehaviour
{
    [SerializeField]
    private string enemyTag = "Enemy";

    [SerializeField]
    private PlayerHealth health;

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == enemyTag)
        {
            //health.ChangeHealth(-1);
        }
    }
}
