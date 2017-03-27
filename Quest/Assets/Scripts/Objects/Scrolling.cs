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
    private TitleManager tm;

    void Start()
    {
        if (PersistentManager.Instance.CurrentScene == PersistentManager.Scenes.Title)
        {
            tm = GameObject.FindGameObjectWithTag("GameController").GetComponent<TitleManager>();
        }
        else
        {
            gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        }
        
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tag != magicTag)
        {
            if (PersistentManager.Instance.CurrentScene == PersistentManager.Scenes.Title)
            {
                transform.position = new Vector2(transform.position.x - (Time.deltaTime * tm.ScrollSpeed), transform.position.y);
            }
            else
            {
                transform.position = new Vector2(transform.position.x - (Time.deltaTime * gm.scrollSpeed), transform.position.y);
            }
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
            if (PersistentManager.Instance.CurrentScene == PersistentManager.Scenes.Title)
            {
                tm.Pool.AddToPool(this.gameObject);
            }
            else
            {
                gm.pool.AddToPool(this.gameObject);
            }
        }
    }

    private void OnBecameInvisible()
    {
        if ((ps != null) || (tag == magicTag) || (tag == barrierTag))
        {
            if (PersistentManager.Instance.CurrentScene != PersistentManager.Scenes.Title)
            {
                gm.pool.AddToPool(gameObject);
            }
        }
        else
        {
            if (PersistentManager.Instance.CurrentScene == PersistentManager.Scenes.Title)
            {
                tm.Pool.AddToPool(gameObject);
            }
        }
    }
}
