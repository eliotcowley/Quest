using UnityEngine;

public class SFXMixer : MonoBehaviour
{
    public AudioClip[] clips;
    
    public enum Sounds
    {
        Coin,
        SwordSwing,
        Heart,
        GhostHit
    }

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(Sounds sound, float delay = 0f)
    {
        audioSource.clip = clips[(int)sound];
        audioSource.PlayDelayed(delay);
    }
}
