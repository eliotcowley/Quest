using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public static string FadeOutAnimatorTrigger = "FadeOut";
    public static string FadeInAnimatorTrigger = "FadeIn";
    public static string SaveDataFilePath = Application.persistentDataPath + "/saveData.dat";
    public static string decorTag = "Decor";
    public static int LevelCount = System.Enum.GetNames(typeof(PersistentManager.Scenes)).Length - (int)PersistentManager.Scenes.Title;
    public static int DiamondCount = System.Enum.GetNames(typeof(Diamond)).Length;
}
