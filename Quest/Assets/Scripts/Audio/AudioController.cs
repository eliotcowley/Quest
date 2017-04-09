using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [HideInInspector]
    public BGM CurrentSong;

    [HideInInspector]
    public AudioSource audioSource;

    [SerializeField]
    private AudioClip[] sceneBGM;

    public enum BGM
    {
        Title = 0
    }

    public void Play(BGM track)
    {
        audioSource.clip = sceneBGM[(int)track];
        audioSource.Play();
        CurrentSong = track;
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
