using UnityEngine;
using System.Collections;

public class Scrolling : MonoBehaviour
{
    [SerializeField]
    private string magicTag = "Magic";

    [SerializeField]
    private string barrierTag = "Barrier";

    private GameManager gm;
    private ParticleSystem ps;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tag != magicTag)
        {
            transform.position = new Vector2(transform.position.x - (Time.deltaTime * gm.scrollSpeed), transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x + (Time.deltaTime * gm.scrollSpeed), transform.position.y);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "GarbageCollector")
        {
            gm.pool.AddToPool(this.gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        if ((ps != null) || (tag == magicTag) || (tag == barrierTag))
        {
            gm.pool.AddToPool(gameObject);
        }
    }
}
