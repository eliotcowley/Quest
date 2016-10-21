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

    [SerializeField]
    private string controllerAButton = "Fire1Controller";

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
        if (Pause.paused) return;
        if (Input.GetButtonDown("Fire1") || (Input.GetButtonDown(controllerAButton)))
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            sfxAudio.clip = sfxMixer.clips[1];
            sfxAudio.PlayDelayed(0.3f);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
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
