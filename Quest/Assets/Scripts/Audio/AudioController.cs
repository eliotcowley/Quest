using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] sceneBGM;

    private AudioSource audioSource;

    public enum BGM
    {
        Title = 0
    }

    public void Play(BGM track)
    {
        audioSource.clip = sceneBGM[(int)track];
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
