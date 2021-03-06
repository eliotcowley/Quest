﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
//using System.Runtime.Serialization.Formatters.Binary;

#if NETFX_CORE
    using XmlReader = WinRTLegacy.Xml.XmlReader;
    using XmlWriter = WinRTLegacy.Xml.XmlWriter;
    using StreamWriter = WinRTLegacy.IO.StreamWriter;
#else
    using XmlReader = System.Xml.XmlReader;
    using XmlWriter = System.Xml.XmlWriter;
#endif

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance;
    public Text loadingText;

    [HideInInspector]
    public Scenes CurrentScene = startScene;

    [HideInInspector]

    [SerializeField]
    private const Scenes startScene = Scenes.TurnOnSound;

    [SerializeField]
    private AudioController audioController;

    [SerializeField]
    private GameObject persistentCamera;

    [SerializeField]
    private string[] sceneNames;

    [SerializeField]
    private SpriteRenderer fader;

    [SerializeField]
    private float smooth = 0.5f;

    [SerializeField]
    private float fadeTime = 1f;

    private bool fadeOut = false;
    private bool fadeIn = false;
    private Color zeroAlpha = new Color(0f, 0f, 0f, 0f);
    private Color fullAlpha = new Color(0f, 0f, 0f, 1f);
    private Scenes sceneToUnload;
    private Scenes sceneToLoad;
    private int[] highScores;
    private BoolArray[] diamonds;
    private AsyncOperation sceneLoadOp;

    public enum Scenes
    {
        Persistent = 0,
        TurnOnSound = 1,
        Title = 2,
        Test = 3,
        Level1_2 = 4
    }

    private void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("ERROR: More than one PersistentManager in scene");
        }
        else
        {
            Instance = this;
        }

        LoadScene(startScene);

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;

        highScores = new int[Constants.LevelCount];

        // Initialize jagged array
        diamonds = new BoolArray[Constants.LevelCount];

        for (int i = 0; i < diamonds.Length; i++)
        {
            diamonds[i] = new BoolArray();
        }

        // TODO: Clear the save data - for debugging, remove later
        //ClearData();
    }

    private void Save()
    {
        //BinaryFormatter bf = new BinaryFormatter();
        XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
        FileStream file = File.Open(Constants.SaveDataFilePath, FileMode.Create);
        //XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
        //settings.Encoding = new System.Text.ASCIIEncoding();
        StreamWriter writer = new StreamWriter(file, Encoding.ASCII);

//#if NETFX_CORE
//        XmlWriter writer = (WinRTLegacy.Xml.XmlWriter)XmlWriter.Create(file, settings);
//#else
//        XmlWriter writer = XmlWriter.Create(file, settings);
//#endif

        SaveData data = new SaveData
        {
            HighScores = highScores,
            Diamonds = diamonds
        };

        //bf.Serialize(file, data);
        //serializer.Serialize(writer, data);
        serializer.Serialize(writer, data);
        //writer.Close();
        writer.Dispose();
        file.Dispose();
    }

    private void Load()
    {
        if (File.Exists(Constants.SaveDataFilePath))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            FileStream file = File.Open(Constants.SaveDataFilePath, FileMode.Open);
            //SaveData data = (SaveData)bf.Deserialize(file);
            SaveData data = (SaveData)serializer.Deserialize(file);
            file.Dispose();

            highScores = data.HighScores;
            diamonds = data.Diamonds;
        }
    }

    public int GetHighScore(int level)
    {
        Load();
        return highScores[level];
    }

    public void SetHighScore(int level, int score)
    {
        highScores[level] = score;
        Save();
    }

    public bool[] GetDiamonds(int level)
    {
        Load();
        bool[] _diamonds = new bool[Constants.DiamondCount];

        for (int i = 0; i < _diamonds.Length; i++)
        {
            _diamonds[i] = diamonds[level].Diamonds[i];
        }

        return _diamonds;
    }

    public void SetDiamonds(int _level, bool[] _diamonds)
    {
        for (int i = 0; i < _diamonds.Length; i++)
        {
            diamonds[_level].Diamonds[i] = _diamonds[i];
        }

        Save();
    }

    private void ClearData()
    {
        //BinaryFormatter bf = new BinaryFormatter();
        XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
        FileStream file = File.Open(Constants.SaveDataFilePath, FileMode.OpenOrCreate);

        SaveData data = new SaveData();

        //bf.Serialize(file, data);
        serializer.Serialize(file, data);
        file.Dispose();
    }

    private void Update()
    {
        if (fadeOut && (fader.color.a < 1f))
        {
            fader.color = Color.Lerp(fader.color, fullAlpha, smooth * Time.deltaTime);
        }
        else if (fadeIn && (fader.color.a > 0f))
        {
            fader.color = Color.Lerp(fader.color, zeroAlpha, smooth * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            ClearData();
        }
    }

    public void LoadScene(Scenes sceneToLoad, Scenes sceneToUnload)
    {
        this.sceneToLoad = sceneToLoad;
        this.sceneToUnload = sceneToUnload;
        FadeOut();
        Invoke("UnloadScene", fadeTime);
    }

    public void LoadScene(Scenes sceneToLoad)
    {
        this.sceneLoadOp = SceneManager.LoadSceneAsync(sceneNames[(int)sceneToLoad], LoadSceneMode.Additive);
        audioController.Stop();

        CurrentScene = sceneToLoad;

        switch (sceneToLoad)
        {
            case Scenes.Title:
                audioController.Play(AudioController.BGM.Title);
                break;
        }
    }

    public int GetLevel()
    {
        switch (CurrentScene)
        {
            case Scenes.Test:
                return 0;
       
            case Scenes.Level1_2:
                return 1;

            default:
                return -1;
        }
    }

    public int GetLevel(Scenes scene)
    {
        return scene - Scenes.Title;
    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        persistentCamera.SetActive(true);
        if (loadingText.gameObject != null) loadingText.gameObject.SetActive(true);
        LoadScene(sceneToLoad);
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        while (this.sceneLoadOp.progress < 0.9f)
        {
            // Spin until scene loaded
        }

        if (scene.name != sceneNames[(int)Scenes.Persistent])
        {
            persistentCamera.SetActive(false);
            StartCoroutine(SetActiveScene(scene));
            loadingText.gameObject.SetActive(false);
        }
    }

    private IEnumerator SetActiveScene(Scene scene)
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.SetActiveScene(scene);
        FadeIn();
    }

    private void FadeOut()
    {
        fader.color = zeroAlpha;
        fadeOut = true;
        fadeIn = false;
    }

    private void FadeIn()
    {
        fadeOut = false;
        fadeIn = true;
    }

    private void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(sceneNames[(int)sceneToUnload]);
    }
}

public enum Diamond
{
    Blue,
    Green,
    Orange
}