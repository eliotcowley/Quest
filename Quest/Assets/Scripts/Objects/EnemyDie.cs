using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EliotScripts.ObjectPool;

public class EnemyDie : MonoBehaviour
{
    [SerializeField]
    private string particlesTag = "Particles";

    [SerializeField]
    private GameObject particlesPrefab;

    [SerializeField]
    private string gameManagerTag = "GameController";

    [SerializeField]
    private string sfxMixerTag = "SFXMixer";

    private ObjectPool pool;
    private SFXMixer sfxMixer;

    private void Start()
    {
        pool = GameObject.FindGameObjectWithTag(gameManagerTag).GetComponent<ObjectPool>();
        sfxMixer = GameObject.FindGameObjectWithTag(sfxMixerTag).GetComponent<SFXMixer>();
    }

    public void Die()
    {
        sfxMixer.PlaySound(SFXMixer.Sounds.GhostKill);
        GameObject particles = pool.PullFromPool(particlesTag);

        if (particles == null)
        {
            Instantiate(particlesPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            particles.transform.position = transform.position;
        }

        pool.AddToPool(gameObject);
    }    
}
