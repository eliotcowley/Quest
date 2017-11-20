using UnityEngine;
using System.Collections;

public class SpawnableObject
{
    public int beatNum;
    public GameObject go;
    public int trackNum;    // 0 = Top, 1 = Middle, 2 = Bottom, 3 = MiddleTop, 4 = MiddleBottom

    public SpawnableObject(int beatNum, GameObject go, int trackNum)
    {
        this.beatNum = beatNum;
        this.go = go;
        this.trackNum = trackNum;
    }
}
