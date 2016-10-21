using UnityEngine;
using System.Collections;

public class AndroidSettings : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseButton;

	// Use this for initialization
	void Start ()
    {
#if UNITY_ANDROID
        pauseButton.SetActive(true);
#endif

    }
}
