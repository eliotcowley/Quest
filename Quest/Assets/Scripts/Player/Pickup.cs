using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    public GameObject blueDiamondUI;
    public GameObject greenDiamondUI;
    public GameObject orangeDiamondUI;

    [SerializeField]
    private GameManager gm;

    [SerializeField]
    private Text coinText;

    [SerializeField]
    private const string coinTag = "Coin";

    [SerializeField]
    private const string heartTag = "Heart";

    [SerializeField]
    private const string starTag = "Star";

    [SerializeField]
    private const string blueDiamondTag = "BlueDiamond";

    [SerializeField]
    private const string greenDiamondTag = "GreenDiamond";

    [SerializeField]
    private const string orangeDiamondTag = "OrangeDiamond";

    [SerializeField]
    private PlayerHealth health;

    private AudioSource sfxAudio;
    private int diamondsThisLevel = 0;

    private PlayerAttack playerAttack;
    private SFXMixer sfxMixer;

    // Use this for initialization
    void Start()
    {
        coinText.text = "0";
        playerAttack = GetComponent<PlayerAttack>();
        sfxMixer = SFXMixer.instance;
        sfxAudio = sfxMixer.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case coinTag:
                gm.coins++;
                gm.pool.AddToPool(other.gameObject);
                coinText.text = gm.coins.ToString();
                sfxAudio.clip = sfxMixer.clips[0];
                sfxAudio.Play();
                break;

            case heartTag:
                health.ChangeHealth(1);
                gm.pool.AddToPool(other.gameObject);
                sfxAudio.clip = sfxMixer.clips[2];
                sfxAudio.Play();
                break;

            case starTag:
                playerAttack.SetInvincible();
                gm.pool.AddToPool(other.gameObject);
                sfxMixer.PlaySound(SFXMixer.Sounds.Star);
                break;

            case blueDiamondTag:
                blueDiamondUI.SetActive(true);
                gm.diamonds[0] = true;
                gm.pool.AddToPool(other.gameObject);
                sfxMixer.PlaySound(SFXMixer.Sounds.Diamond);
                diamondsThisLevel++;
                break;

            case greenDiamondTag:
                greenDiamondUI.SetActive(true);
                gm.diamonds[1] = true;
                gm.pool.AddToPool(other.gameObject);
                sfxMixer.PlaySound(SFXMixer.Sounds.Diamond);
                diamondsThisLevel++;
                break;

            case orangeDiamondTag:
                orangeDiamondUI.SetActive(true);
                gm.diamonds[2] = true;
                gm.pool.AddToPool(other.gameObject);
                sfxMixer.PlaySound(SFXMixer.Sounds.Diamond);
                diamondsThisLevel++;
                break;
        }
    }
}
