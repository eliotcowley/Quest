using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class SongData
{
    public List<AnalysisData> analyses;

    public BeatTracker beatTracker;
    public Segmenter segmenter;

    public string name;
    public int length;

    public SongData(List<AnalysisData> analyses, string name, int length, BeatTracker beatTracker, Segmenter segmenter)
    {
        this.analyses = analyses;
        this.name = name;
        this.length = length;
        this.segmenter = segmenter;
        this.beatTracker = beatTracker;
    }

    public void Serialize()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + name + ".rthm", FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, this);

        stream.Close();
    }

    public static SongData Deserialize(string name)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + name + ".rthm", FileMode.Open, FileAccess.Read, FileShare.Read);
        SongData obj = (SongData)formatter.Deserialize(stream);
        stream.Close();

        return obj;
    }
}
