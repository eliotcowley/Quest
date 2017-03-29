using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PixelPerfectCamera : MonoBehaviour
{
    [SerializeField]
    private float pixelsToUnits = 1f;

    private Camera myCamera;

    private void Start()
    {
        myCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        myCamera.orthographicSize = Screen.height / pixelsToUnits / 2;
    }
}
