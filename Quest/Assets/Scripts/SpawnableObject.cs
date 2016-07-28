using UnityEngine;
using System.Collections;

public class SpawnableObject
{
    public float beatNum;
    public GameObject go;
    public int trackNum;

    public SpawnableObject(float beatNum, GameObject go, int trackNum)
    {
        this.beatNum = beatNum;
        this.go = go;
        this.trackNum = trackNum;
    }
}
