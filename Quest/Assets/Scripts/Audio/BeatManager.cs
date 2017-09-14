using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
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

    private void OnReadyToPlay()
    {
        if (rhythmTool.songLoaded)
        {
            Debug.Log("Song loaded");
        }

        StartCoroutine(PlaySong());
    }

    private IEnumerator PlaySong()
    {
        yield return new WaitForSeconds(1f);
        rhythmTool.Play();
    }
}
