using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager instance;

    [SerializeField]
    private const Scenes startScene = Scenes.Title;

    [SerializeField]
    private AudioController audioController;

    [SerializeField]
    private GameObject persistentCamera;

    [SerializeField]
    private string[] sceneNames;

    public enum Scenes
    {
        Title = 0,
        Test = 1,
        Persistent = 2
    }

    public void LoadScene(Scenes sceneToLoad, Scenes sceneToUnload)
    {
        SceneManager.UnloadSceneAsync(sceneNames[(int)sceneToUnload]);
        LoadScene(sceneToLoad);
    }

    public void LoadScene(Scenes sceneToLoad)
    {
        SceneManager.LoadSceneAsync(sceneNames[(int)sceneToLoad], LoadSceneMode.Additive);
        audioController.Stop();

        switch (sceneToLoad)
        {
            case startScene:
                audioController.Play(AudioController.BGM.Title);
                break;
        }
    }

    private void Start()
    {
        LoadScene(startScene);

        if (instance != null)
        {
            Debug.LogError("ERROR: More than one PersistentManager in scene");
        }
        else
        {
            instance = this;
        }

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        persistentCamera.SetActive(true);
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name != sceneNames[(int)Scenes.Persistent])
        {
            persistentCamera.SetActive(false);
            StartCoroutine(SetActiveScene(scene));
        }
    }

    private IEnumerator SetActiveScene(Scene scene)
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.SetActiveScene(scene);
    }
}
