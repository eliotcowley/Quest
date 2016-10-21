using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    private GameManager gm;

    [SerializeField]
    private Text coinText;

    [SerializeField]
    private SFXMixer sfxMixer;

    [SerializeField]
    private const string coinTag = "Coin";

    [SerializeField]
    private const string heartTag = "Heart";

    private AudioSource sfxAudio;

    [SerializeField]
    private PlayerHealth health;

    // Use this for initialization
    void Start()
    {
        coinText.text = "0";
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
        }
    }
}
