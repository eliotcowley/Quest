using UnityEngine;
using EliotScripts.ObjectPool;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public string particlesTag = "Particles";
    public GameObject particlesPrefab;
    public float hurtFlashTime = 1f;
    public int hurtFlashes = 2;
    public int invincibleBeats = 16;

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
    private Material material;
    private WaitForSeconds hurtFlashSeconds;
    private bool isInvincible = false;
    private ParticleSystem invincibleParticles;
    private int beatCount;

    private void Start()
    {
        isAttacking = false;
        animator = GetComponent<Animator>();
        sfxAudio = sfxMixer.GetComponent<AudioSource>();
        material = GetComponent<Renderer>().material;
        hurtFlashSeconds = new WaitForSeconds(hurtFlashTime);
        invincibleParticles = GetComponentInChildren<ParticleSystem>();
        beatCount = invincibleBeats;
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
            sfxMixer.PlaySound(SFXMixer.Sounds.SwordSwing, 0.3f);
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
            if (isAttacking || isInvincible)
            {
                pool.AddToPool(other.gameObject);
                sfxMixer.PlaySound(SFXMixer.Sounds.GhostKill);
                GameObject particles = pool.PullFromPool(particlesTag);

                if (particles == null)
                {
                    Instantiate(particlesPrefab, other.transform.position, Quaternion.identity);
                }
                else
                {
                    particles.transform.position = other.transform.position;
                }
            }
            else
            {
                health.ChangeHealth(-1);
                sfxMixer.PlaySound(SFXMixer.Sounds.GhostHit);
                if (gameObject.activeSelf)
                {
                    StartCoroutine(FlashRed());
                }
            }
        }
    }

    private IEnumerator FlashRed()
    {
        int i = hurtFlashes;
        while (i > 0)
        {
            material.color = Color.red;
            yield return hurtFlashSeconds;
            material.color = Color.white;
            yield return hurtFlashSeconds;
            i--;
        }
    }

    public void SetInvincible()
    {
        isInvincible = true;
        material.color = Color.yellow;
        invincibleParticles.Play();
    }

    public void SetNormal()
    {
        isInvincible = false;
        material.color = Color.white;
        invincibleParticles.Stop();
        beatCount = invincibleBeats;
    }

    public void OnBeat()
    {
        if (isInvincible)
        {
            if (beatCount <= 0)
            {
                SetNormal();
            }
            else
            {
                beatCount--;
            }
        }
    }
}
