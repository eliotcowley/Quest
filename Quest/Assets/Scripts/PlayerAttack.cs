using UnityEngine;
using System.Collections;
using EliotScripts.ObjectPool;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private SFXMixer sfxMixer;

    [SerializeField]
    private string enemyTag = "Enemy";

    [SerializeField]
    private ObjectPool pool;

    [SerializeField]
    private PlayerHealth health;

    private bool isAttacking;
    private Animator animator;
    private AudioSource sfxAudio;

    private void Start()
    {
        isAttacking = false;
        animator = GetComponent<Animator>();
        sfxAudio = sfxMixer.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("Attack");
                sfxAudio.clip = sfxMixer.clips[1];
                sfxAudio.PlayDelayed(0.3f);
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == enemyTag)
        {
            if (isAttacking)
            {
                pool.AddToPool(other.gameObject);
            }
            else
            {
                health.ChangeHealth(-1);
            }
        }
    }
}
