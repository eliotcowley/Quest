using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
class SaveData
{
    public int[] HighScores;
    public bool[,] Diamonds; // [Level, Diamond]

    public SaveData()
    {
        HighScores = new int[Constants.LevelCount];
        Diamonds = new bool[Constants.LevelCount, Constants.DiamondCount];
    }
}
