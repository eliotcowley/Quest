using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    public static List<List<bool>> InitJagged2DBoolArray(int rows, int cols)
    {
        List<List<bool>> array = new List<List<bool>>();

        for (int i = 0; i < rows; i++)
        {
            array.Add(new List<bool>());

            for (int j = 0; j < cols; j++)
            {
                array[i].Add(false);
            }
        }

        return array;
    }

    public static BoolArray[] InitDiamondArray()
    {
        BoolArray[] array = new BoolArray[Constants.LevelCount];

        for (int i = 0; i < array.Length; i++)
        {
            array[i].Diamonds = new bool[Constants.DiamondCount];
        }

        return array;
    }
}
