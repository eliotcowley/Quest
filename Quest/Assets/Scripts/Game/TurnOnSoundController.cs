using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnSoundController : MonoBehaviour
{
    [SerializeField]
    private float transitionTime = 2f;

    private void Start()
    {
        StartCoroutine(GoToTitleScreen());   
    }

    private IEnumerator GoToTitleScreen()
    {
        yield return new WaitForSeconds(transitionTime);
        PersistentManager.Instance.LoadScene(PersistentManager.Scenes.Title, PersistentManager.Instance.CurrentScene);
    }
}
