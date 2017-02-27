using UnityEngine;

public class SFXMixer : MonoBehaviour
{
    public static SFXMixer instance;
    public AudioClip[] clips;
    
    public enum Sounds
    {
        Coin,
        SwordSwing,
        Heart,
        GhostHit,
        PlayerDie,
        GhostKill,
        Star,
        Diamond,
        Magic
    }

    private AudioSource[] audioSources;

    private void Start()
    {
        audioSources = GetComponents<AudioSource>();

        if (instance != null)
        {
            Debug.LogError("ERROR: More than one SFXMixer in scene");
        }
        else
        {
            instance = this;
        }
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
