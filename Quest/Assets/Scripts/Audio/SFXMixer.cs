using UnityEngine;

public class SFXMixer : MonoBehaviour
{
    public AudioClip[] clips;
    
    public enum Sounds
    {
        Coin,
        SwordSwing,
        Heart,
        GhostHit,
        PlayerDie,
        GhostKill
    }

    private AudioSource[] audioSources;

    private void Start()
    {
        audioSources = GetComponents<AudioSource>();
    }

    public void PlaySound(Sounds sound, float delay = 0f)
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = clips[(int)sound];
                audioSource.PlayDelayed(delay);
                break;
            }
        }
    }
}
