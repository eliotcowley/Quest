using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PickupCoin : MonoBehaviour
{
    [SerializeField]
    private GameManager gm;
    [SerializeField]
    private Text coinText;
    [SerializeField]
    private SFXMixer sfxMixer;

    private AudioSource sfxAudio;

    // Use this for initialization
    void Start()
    {
        coinText.text = "0";
        sfxAudio = sfxMixer.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Coin")
        {
            gm.coins++;
            gm.pool.AddToPool(other.gameObject);
            coinText.text = gm.coins.ToString();
            sfxAudio.clip = sfxMixer.clips[0];
            sfxAudio.Play();
        }
    }
}
