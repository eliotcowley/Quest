using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private RhythmTool rhythmTool;
    private AudioClip audioClip;

    // Use this for initialization
    void Start()
    {
        rhythmTool = GetComponent<RhythmTool>();
        audioClip = GetComponent<AudioSource>().clip;

        rhythmTool.NewSong(audioClip);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnReadyToPlay()
    {
        if (rhythmTool.songLoaded)
        {
            Debug.Log("Song loaded");
        }
        rhythmTool.Play();
    }
}
