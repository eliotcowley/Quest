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
    //Rigidbody2D rb2d;

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
        //rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tag != magicTag)
        {
            if (tag == "Particles")
            {
                if (!ps.IsAlive())
                {
                    AddToPool();
                }
            }
            if (PersistentManager.Instance.CurrentScene == PersistentManager.Scenes.Title)
            {
                transform.position = new Vector2(transform.position.x - (Time.deltaTime * tm.ScrollSpeed), transform.position.y);
            }
            else
            {
                transform.position = new Vector2(transform.position.x - (Time.deltaTime * gm.scrollSpeed), transform.position.y);
                //rb2d.MovePosition(new Vector2(rb2d.position.x - (Time.deltaTime * gm.scrollSpeed), rb2d.position.y));
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
            AddToPool();
        }
    }

    private void AddToPool()
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

    //private void OnBecameInvisible()
    //{
    //    if ((ps != null) || (tag == magicTag) || (tag == barrierTag))
    //    {
    //        if (PersistentManager.Instance.CurrentScene != PersistentManager.Scenes.Title)
    //        {
    //            gm.pool.AddToPool(gameObject);
    //        }
    //    }
    //    else
    //    {
    //        if (PersistentManager.Instance.CurrentScene == PersistentManager.Scenes.Title)
    //        {
    //            tm.Pool.AddToPool(gameObject);
    //        }
    //    }
    //}
}
