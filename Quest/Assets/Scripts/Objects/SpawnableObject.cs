using UnityEngine;
using System.Collections;

public class SpawnableObject
{
    public int beatNum;
    public GameObject go;
    public int trackNum;

    public SpawnableObject(int beatNum, GameObject go, int trackNum)
    {
        this.beatNum = beatNum;
        this.go = go;
        this.trackNum = trackNum;
    }
}
