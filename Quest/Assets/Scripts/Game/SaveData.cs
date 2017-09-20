using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Linq;

[Serializable]
public class SaveData
{
    public int[] HighScores;

    public BoolArray[] Diamonds; // [Level, Diamond]

    public SaveData()
    {
        HighScores = new int[Constants.LevelCount];
        Diamonds = new BoolArray[Constants.LevelCount];

        for (int i = 0; i < Diamonds.Length; i++)
        {
            Diamonds[i] = new BoolArray();
            Diamonds[i].Diamonds = new bool[Constants.DiamondCount];
        }
    }
}

public class BoolArray
{
    public bool[] Diamonds;

    public BoolArray()
    {
        Diamonds = new bool[Constants.DiamondCount];
    }
}
