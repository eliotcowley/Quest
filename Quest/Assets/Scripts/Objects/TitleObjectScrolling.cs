using EliotScripts.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleObjectScrolling : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed;

    [SerializeField]
    private ObjectPool pool;

    private void Update()
    {
        transform.position = new Vector2(
            transform.position.x + (Time.deltaTime * scrollSpeed), 
            transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "GarbageCollector")
        {
            pool.AddToPool(this.gameObject);
        }
    }
}
