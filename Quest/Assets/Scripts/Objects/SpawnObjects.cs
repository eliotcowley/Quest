using UnityEngine;
using System.Collections;
using EliotScripts.ObjectPool;

public class SpawnObjects : MonoBehaviour
{
    [SerializeField]
    private Transform[] positions;
    
    [SerializeField]
    private ObjectPool pool;

    [SerializeField]
    private float startTime = 8.5f;

    [SerializeField]
    private float beatLength;

    [SerializeField]
    private AudioSource music;

    [SerializeField]
    private int bpm = 92;

    [SerializeField]
    private int startBeat = 88;

    [SerializeField]
    private float startDelay = 1f;

    private float nextTime;
    private int nextPosition;
    private int index;
    private GameObject nextPrefab;
    private int nextSample;
    private float currentSample;
    private int currentBeat = 0;
    private int nextBeat;

    public GameObject coinPrefab;
    public GameObject ghostPrefab;
    public GameObject heartPrefab;

    private void Start()
    {
        //double initTime = AudioSettings.dspTime;
        //music.PlayScheduled(initTime + startDelay);

        //nextTime = startTime;
        index = 0;
        //InitSpawn();
        //nextSample = startBeat * bpm;
        //music.Play();
        nextBeat = startBeat;
    }

    private void Update()
    {
        //currentSample = (float)AudioSettings.dspTime * music.clip.frequency;
        //if (currentSample >= nextSample)
        //{
        //    Spawn();
        //}
        //Debug.Log(music.timeSamples);
    }


    public void Spawn()
    {
        currentBeat++;
        if (!SpawnableObjectList.prefabsSet || (index >= SpawnableObjectList.objects.Length) || (currentBeat < nextBeat))
        {
            return;
        }

        GameObject prefab;
        nextPosition = SpawnableObjectList.objects[index].trackNum;
        nextPrefab = SpawnableObjectList.objects[index++].go;

        if (nextPrefab == null)
        {
            Debug.LogError("No prefab!");
        }

        GameObject obj;

        switch(nextPrefab.tag)
        {
            case "Coin":
                prefab = pool.PullFromPool("Coin");
                obj = coinPrefab;
                break;

            case "Enemy":
                prefab = pool.PullFromPool("Enemy");
                obj = ghostPrefab;
                break;

            case "Heart":
                prefab = pool.PullFromPool("Heart");
                obj = heartPrefab;
                break;

            default:
                prefab = pool.PullFromPool("Coin");
                obj = coinPrefab;
                break; 
        }
        if (prefab == null)
        {
            Instantiate(obj, positions[nextPosition].position, Quaternion.identity);
        }
        else
        {
            prefab.transform.position = positions[nextPosition].position;
        }
        if (index < SpawnableObjectList.objects.Length)
        {
            //nextTime = SpawnableObjectList.objects[index].beatNum * beatLength;
            //Invoke("Spawn", nextTime);
            //nextSample += SpawnableObjectList.objects[index].beatNum * bpm;
            nextBeat = SpawnableObjectList.objects[index].beatNum;
        }
        //Debug.Log("Audio Sample: " + music.timeSamples);
        currentBeat = 0;
    }

    private void InitSpawn()
    {
        Invoke("Spawn", nextTime);
        music.Play();
    }
}
