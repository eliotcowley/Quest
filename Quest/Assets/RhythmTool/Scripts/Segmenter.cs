using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[System.Serializable]
public class Segmenter {

    [System.NonSerialized]
    private AnalysisData analysis;
    
    private float lastDif;
    private int increaseStart;
    private int increaseEnd;
    private bool increaseDetected;
    private int decreaseStart;
    private int decreaseEnd;
    private bool decreaseDetected;

    private Dictionary<int, float> _changes;
    public ReadOnlyDictionary<int, float> changes { get; private set; }

    private List<int> _changeIndices;
    public ReadOnlyCollection<int> changeIndices { get; private set; }

    public Segmenter(AnalysisData analysis)
    {
        this.analysis = analysis;
        _changes = new Dictionary<int, float>();
        _changeIndices = new List<int>();

        changes = new ReadOnlyDictionary<int, float>(_changes);
        changeIndices = _changeIndices.AsReadOnly();
    }

    public void Init()
    {
        lastDif = 0;
        increaseStart = 0;
        increaseEnd = 0;
        increaseDetected = false;
        decreaseStart = 0;
        decreaseEnd = 0;
        decreaseDetected = false;

        _changes.Clear();        
        _changeIndices.Clear();
    }

    /// <summary>
    /// Initialize with existing data.
    /// </summary>
    /// <param name="data"></param>
    public void Init(SongData data)
    {
        _changeIndices.Clear();
        _changeIndices.AddRange(data.segmenter.changeIndices);

        _changes.Clear();
        foreach (KeyValuePair<int, float> change in data.segmenter.changes)
        {
            _changes.Add(change.Key, change.Value);
        }
    }

    /// <summary>
    /// Looks at consecutive frames and tries to find overall increases and decreases in a song's loudness
    /// </summary>
    /// <param name="index"></param>
    public void DetectChanges(int index)
    {
        if (index < 0)
            return;

        float a = analysis.magnitudeAvg[index + 1];
        float b = analysis.magnitudeAvg[index];

        float dif = a - b;

        if (dif >= .05f && lastDif < .05f && !increaseDetected)
        {
            increaseStart = index;
            increaseDetected = true;
        }
        if (dif <= -.08f && lastDif > -.08f && !decreaseDetected)
        {
            decreaseStart = index;
            decreaseDetected = true;
        }

        if (dif < .04f && lastDif > .04f && increaseDetected)
        {
            float rl = 22;

            if (dif > -.04f)
                rl = 12;

            increaseEnd = index;

            increaseDetected = false;

            float steepest = 0;
            int si = increaseStart;
            
            for (int i = increaseStart; i < increaseEnd; i++)
            {
                float a2 = (analysis.magnitudeSmooth[i + 1]);
                float b2 = (analysis.magnitudeSmooth[i]);

                float nextDif = a2 - b2;

                if (nextDif > steepest)
                {
                    steepest = nextDif;
                    si = i;
                }
            }

            //TODO: possibly align with nearest and biggest onset near si (steepest index). if there is none, do the usual

            a = analysis.magnitudeAvg[increaseEnd];
            b = analysis.magnitudeAvg[increaseStart];

            float length = Mathf.Sqrt(Mathf.Pow((a - b), 2) + Mathf.Pow((increaseEnd * .1f - increaseStart * .1f), 2));

            if (length > rl)
            {
                _changes.Add(si, a);
                _changeIndices.Add(si);
            }
        }

        if (dif > -.04f && lastDif < -.04f && decreaseDetected)
        {
            float rl = 22;

            if (dif < .04f)
                rl = 15;

            decreaseEnd = index;
            decreaseDetected = false;

            float steepest = 0;
            int si = decreaseStart;

            for (int i = decreaseStart; i < decreaseEnd; i++)
            {
                float a2 = (analysis.magnitudeSmooth[i + 1]);
                float b2 = (analysis.magnitudeSmooth[i]);

                float nextDif = a2 - b2;

                if (nextDif < steepest)
                {
                    steepest = nextDif;
                    si = i;
                }
            }

            a = analysis.magnitudeAvg[decreaseEnd];
            b = analysis.magnitudeAvg[decreaseStart];

            float length = Mathf.Sqrt(Mathf.Pow((a - b), 2) + Mathf.Pow((decreaseEnd * .1f - decreaseStart * .1f), 2));

            if (length > rl)
            {
                _changes.Add(si, -a);
                _changeIndices.Add(si);
            }
        }

        lastDif = dif;
    }

    /// <summary>
    /// does a change occur at index?
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool IsChange(int index)
    {
        if (_changes.Count == 0)
            return false;

        int nextChange = NextChangeIndex((int)index);

        if (nextChange == index)
            return true;

        return false;
    }

    /// <summary>
    /// The index of the closest change preceding index	
    /// </summary>
    /// <returns>The change index.</returns>
    /// <param name="index">Index.</param>
    public int PrevChangeIndex(int index)
    {
        if (_changeIndices.Count == 0)
            return 0;

        int prevChange = _changeIndices.BinarySearch(index);
        prevChange = Mathf.Max(prevChange, ~prevChange);
        prevChange = Mathf.Clamp(prevChange - 1, 0, _changeIndices.Count - 1);

        prevChange = _changeIndices[prevChange];
        return prevChange;
    }

    /// <summary>
    /// The index of the closest change following index	
    /// </summary>
    /// <returns>The change index.</returns>
    /// <param name="index">Index.</param>
    public int NextChangeIndex(int index)
    {
        if (_changeIndices.Count == 0)
            return 0;

        int nextChange = _changeIndices.BinarySearch(index);
        nextChange = Mathf.Max(nextChange, ~nextChange);
        nextChange = Mathf.Clamp(nextChange, 0, _changeIndices.Count - 1);

        nextChange = _changeIndices[nextChange];
        return nextChange;
    }

    /// <summary>
	/// The closest change preceding index	
	/// </summary>
	/// <returns>The change.</returns>
	/// <param name="index">Index.</param>
	public float PrevChange(int index)
    {
        if (_changes.Count == 0)
            return 0;

        int prevChange = PrevChangeIndex(index);

        return _changes[prevChange];
    }

    /// <summary>
	/// The closest change following index	
	/// </summary>
	/// <returns>The change.</returns>
	/// <param name="index">Index.</param>
	public float NextChange(int index)
    {
        if (_changes.Count == 0)
            return 0;

        int nextChange = NextChangeIndex(index);

        return _changes[nextChange];
    }
}
