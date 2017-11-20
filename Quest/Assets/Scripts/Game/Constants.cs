using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public static string FadeOutAnimatorTrigger = "FadeOut";
    public static string FadeInAnimatorTrigger = "FadeIn";
    public static string SaveDataFilePath = Application.persistentDataPath + "/saveData.dat";

    // Tags
    public static string DecorTag = "Decor";
    public static string CoinTag = "Coin";
    public static string EnemyTag = "Enemy";
    public static string HeartTag = "Heart";
    public static string StarTag = "Star";
    public static string BlueDiamondTag = "BlueDiamond";
    public static string GreenDiamondTag = "GreenDiamond";
    public static string OrangeDiamondTag = "OrangeDiamond";
    public static string BarrierTag = "Barrier";
    public static string GoalTag = "Goal";
    public static string SnailTag = "Snail";
    public static string MushroomTag = "Mushroom";

    public static int LevelCount = 
        System.Enum.GetNames(typeof(PersistentManager.Scenes)).Length - (int)PersistentManager.Scenes.Title;

    public static int DiamondCount = System.Enum.GetNames(typeof(Diamond)).Length;
}
