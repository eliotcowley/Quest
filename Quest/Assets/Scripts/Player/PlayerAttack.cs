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

    [HideInInspector]
    public static bool isAttacking;

    [HideInInspector]
    public bool swordsmanInFront;

    [SerializeField]
    private string enemyTag = "Enemy";

    [SerializeField]
    private ObjectPool pool;

    [SerializeField]
    private PlayerHealth health;

    [SerializeField]
    private string controllerAButton = "Fire1Controller";

    [SerializeField]
    private GameObject swordsman;

    [SerializeField]
    private GameObject princess;

    [SerializeField]
    private string magicTag = "Magic";

    [SerializeField]
    private GameObject magicPrefab;

    [SerializeField]
    private float magicRate = 0.5f;

    private Animator swordsmanAnimator;
    private Animator princessAnimator;
    private Material swordsmanMaterial;
    private Material princessMaterial;
    private WaitForSeconds hurtFlashSeconds;
    private bool isInvincible = false;
    private ParticleSystem invincibleParticles;
    private int beatCount;
    private bool canUseMagic = true;
    private SFXMixer sfxMixer;

    private void Start()
    {
        isAttacking = false;
        swordsmanAnimator = swordsman.GetComponent<Animator>();
        princessAnimator = princess.GetComponent<Animator>();
        swordsmanMaterial = swordsman.GetComponent<Renderer>().material;
        princessMaterial = princess.GetComponent<Renderer>().material;
        hurtFlashSeconds = new WaitForSeconds(hurtFlashTime);
        invincibleParticles = GetComponentInChildren<ParticleSystem>();
        beatCount = invincibleBeats;
        swordsmanInFront = true;
        sfxMixer = SFXMixer.instance;
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
        if (swordsmanInFront && !isAttacking)
        {
            isAttacking = true;
            swordsmanAnimator.SetTrigger("Attack");
            sfxMixer.PlaySound(SFXMixer.Sounds.SwordSwing, 0.3f);
        }
        else if (!swordsmanInFront && canUseMagic)
        {
            // Uncomment this when I have animations for the princess.
            //princessAnimator.SetTrigger("Attack");
            sfxMixer.PlaySound(SFXMixer.Sounds.Magic);
            GameObject magic = pool.PullFromPool(magicTag);

            if (magic == null)
            {
                magic = Instantiate(magicPrefab);
            }

            magic.transform.position = this.transform.position;
            canUseMagic = false;
            Invoke("CanUseMagic", magicRate);
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
            if ((isAttacking && swordsmanInFront) || isInvincible)
            {
                other.gameObject.GetComponent<EnemyDie>().Die();
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
            swordsmanMaterial.color = Color.red;
            princessMaterial.color = Color.red;
            yield return hurtFlashSeconds;
            swordsmanMaterial.color = Color.white;
            princessMaterial.color = Color.white;
            yield return hurtFlashSeconds;
            i--;
        }
    }

    public void SetInvincible()
    {
        isInvincible = true;
        swordsmanMaterial.color = Color.yellow;
        princessMaterial.color = Color.yellow;
        invincibleParticles.Play();
    }

    public void SetNormal()
    {
        isInvincible = false;
        swordsmanMaterial.color = Color.white;
        princessMaterial.color = Color.white;
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

    private void CanUseMagic()
    {
        canUseMagic = true;
    }
}
