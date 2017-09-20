using UnityEngine;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

using System.Threading;

#if NETFX_CORE
using Windows.System.Threading;
#endif

/// <summary>
/// Class that tracks beats and finds BPM
/// </summary>
[System.Serializable]
public class BeatTracker
{    
    /// <summary>
    /// Buffer that stores the incoming data
    /// </summary>
    private float[] signalBuffer;
    /// <summary>
    /// Array that stores the signal that's being analyzed at that moment
    /// </summary>
    private float[] currentSignal;
    /// <summary>
    /// Score for how similar the delayed signal is to itself
    /// </summary>
    private float[] repetitionScore;
    /// <summary>
    /// Score for how much a given gap occurs in the signal
    /// </summary>
    private float[] gapScore;
    /// <summary>
    /// Score for which offset is the most likely when the previously found beat length is compared with the original signal
    /// </summary>
    private float[] offsetScore;

    /// <summary>
    /// List of peaks found in repetitionScore
    /// </summary>
    private List<int> peaks = new List<int>();
    /// <summary>
    /// Highest value in repetitionScore
    /// </summary>
    private int bestRepetition = 0;

    /// <summary>
    /// The last found beat length
    /// </summary>
    private int currentBeatLength = 0;

    private float syncOld;

    /// <summary>
    /// Number of frames between updating the beat tracking
    /// </summary>
    private int beatTrackInterval = 20;

    /// <summary>
    /// index of the last processed input
    /// </summary>
    private int index = 0;

    /// <summary>
	/// Indices of detected beats for quick binarysearch.
	/// </summary>
	private List<int> _beatIndices;
    public ReadOnlyCollection<int> beatIndices { get; private set; }

    /// <summary>
    /// Beats. Key is the index of the frame. Value is Beat. 
    /// </summary>
    private Dictionary<int, Beat> _beats;
    public ReadOnlyDictionary<int, Beat> beats { get; private set; }

    /// <summary>
    /// Used to keep track of and ajust synchronization
    /// </summary>
    private float sync = 0;

    /// <summary>
    /// Histogram of how many times each beat length is counted.
    /// Most counted will likely be the correct beat length.
    /// </summary>
    private float[] beatHistogram;

    /// <summary>
    /// Histogram of how many times each BPM value is counted.
    /// The most frequent value will likely be the correct BPM.
    /// </summary>
    private int[] bpmHistogram;

    /// <summary>
    /// The length of one sample
    /// </summary>
    private float frameLength;
        
    public BeatTracker()
    {
        _beatIndices = new List<int>();
        _beats = new Dictionary<int, Beat>();

        beatIndices = _beatIndices.AsReadOnly();
        beats = new ReadOnlyDictionary<int, Beat>(_beats);
    }

    
    /// <summary>
    /// Initialize for a new song
    /// </summary>
    /// <param name="frameLength">The length of a frame for this song</param>            
    public void Init(float frameLength)
    {
        this.frameLength = frameLength;

        beatHistogram = new float[91];
        bpmHistogram = new int[200];
        signalBuffer = new float[310];

        repetitionScore = new float[600];
        currentSignal = new float[signalBuffer.Length];
        gapScore = new float[180];
        offsetScore = new float[100];

        sync = 0;
        index = 0;
        syncOld = 0;
        bestRepetition = 0;
        currentBeatLength = 0;

        _beatIndices.Clear();
        _beats.Clear();
    }

    /// <summary>
    /// Initialize with existing data.
    /// </summary>
    /// <param name="data"></param>
    public void Init(SongData data)
    {
        _beatIndices.Clear();
        _beatIndices.AddRange(data.beatTracker.beatIndices);

        _beats.Clear();
        foreach (KeyValuePair<int, Beat> beat in data.beatTracker.beats)
        {
            _beats.Add(beat.Key, beat.Value);
        }
    }

    public void FillStart()
    {
        if (_beatIndices.Count < 10)
            return;

        Dictionary<float, int> lengths = new Dictionary<float, int>();

        int max = Mathf.Min(20, _beatIndices.Count);

        for (int i = 0; i < max; i++)
        {
            int ii = _beatIndices[i];
            float length = _beats[ii].length;

            if (lengths.ContainsKey(length))
                lengths[length]++;
            else
                lengths.Add(length, 1);
        }

        int beatCount = 0;
        float beatLength = 0;

        foreach (KeyValuePair<float, int> l in lengths)
        {
            if (l.Value > beatCount)
            {
                beatCount = l.Value;
                beatLength = l.Key;
            }
        }

        if (beatLength < 1)
            return;

        float firstIndex = _beatIndices[0];

        for (int i = 0; i < max; i++)
        {
            if (_beats[_beatIndices[i]].length == beatLength)
            {
                firstIndex = _beatIndices[i];
                break;
            }
        }

        List<int> toRemove = new List<int>();

        foreach(KeyValuePair<int, Beat> k in _beats)
        {
            if(k.Value.index < firstIndex)
            {
                toRemove.Add(k.Key);
            }
        }

        foreach(int k in toRemove)
        {
            _beatIndices.Remove(k);
            _beats.Remove(k);
        }

        float bpm = _beats[beatIndices[beatIndices.Count-1]].bpm;

        while(firstIndex > beatLength)
        {
            firstIndex -= beatLength;

            int index = Mathf.RoundToInt(firstIndex);

            _beatIndices.Insert(0, index);
            _beats.Add(index, new Beat(beatLength, bpm, index));
        }
    }

    /// <summary>
    /// Fills beats at the end of the song based on best last data.
    /// </summary>
    public void FillEnd()
    {
        if (_beatIndices.Count < 40)
            return;

        Dictionary<float, int> lengths = new Dictionary<float, int>();

        for (int i = _beatIndices.Count - 30; i < _beatIndices.Count; i++)
        {
            int ii = _beatIndices[i];
            float length = _beats[ii].length;

            if (lengths.ContainsKey(length))
                lengths[length]++;
            else
                lengths.Add(length, 1);
        }

        int beatCount = 0;
        float beatLength = 0;

        foreach (KeyValuePair<float, int> l in lengths)
        {
            if (l.Value > beatCount)
            {
                beatCount = l.Value;
                beatLength = l.Key;
            }
        }

        int lastBeat = _beatIndices[_beatIndices.Count - 1];

        int numFill = (int)((index - lastBeat) / beatLength);

        for (int i = 1; i < numFill; i++)
        {
            _beatIndices.Add((i * (int)beatLength) + lastBeat);
            AddBeat((i * (int)beatLength) + lastBeat, beatLength);
        }
    }

    /// <summary>
    /// Adds a new beat and updates BPM.
    /// </summary>
    /// <param name="index">Index of beat</param>
    /// <param name="beatLength">Beat length in frames</param>
    private void AddBeat(int index, float beatLength)
    {
        _beatIndices.Add(index);

        float cBpm = 0;
        int c = 0;
        for (int i = Mathf.Max(1, _beatIndices.Count - 11); i < _beatIndices.Count; i++)
        {
            cBpm += _beatIndices[i] - _beatIndices[i - 1];
            c++;
        }

        if (c == 0)
            cBpm = (60 / (frameLength * beatLength));
        else
        {
            cBpm /= c;
            cBpm = (60 / (frameLength * cBpm));
        }

        cBpm = Mathf.Round(cBpm);
        cBpm = Mathf.Clamp(cBpm, 0, 199);

        bpmHistogram[Mathf.RoundToInt(cBpm)]++;

        int l = 0;

        for (int i = 0; i < bpmHistogram.Length; i++)
        {
            if (bpmHistogram[i] > bpmHistogram[l])
                l = i;
        }

        if (bpmHistogram[l] > 20)
        {
            for (int i = 0; i < bpmHistogram.Length; i++)
            {
                bpmHistogram[i] = Mathf.Max(0, bpmHistogram[i] - 10);
            }
        }

        Beat b = new Beat(beatLength, l, index);
        _beats.Add(index, b);
    }

    /// <summary>
    /// Looks at consecutive frames and tracks beat.
    /// A part of the song is compared to itself to determine repetition in the song.
    /// Most likely repetition is chosen. This interval is then synced with the song.
    /// </summary>
    /// <param name="sample">One sample of a signal. Samples should be fed one by one.</param>
    public void TrackBeat(float sample)
    {
        for (int i = 0; i < signalBuffer.Length - 1; i++)
        {
            signalBuffer[i] = signalBuffer[i + 1];
        }

        signalBuffer[signalBuffer.Length - 1] = sample;

        if (index > currentSignal.Length && index % beatTrackInterval == 0)
        {
            for (int i = 0; i < currentSignal.Length; i++)
            {
                currentSignal[i] = signalBuffer[i];
            }

            syncOld = sync;

#if NETFX_CORE
            Windows.System.Threading.ThreadPool.RunAsync(o => FindBeat());
#else
            ThreadPool.QueueUserWorkItem(o => FindBeat());
#endif
        }

        if (currentBeatLength > 5)
        {
            if (currentBeatLength > 20)
            {
                if (sync > currentBeatLength)
                {
                    sync -= currentBeatLength;
                    AddBeat(index - currentSignal.Length + 5, currentBeatLength / 4f);
                }
            }
            sync += 4;
        }
        index++;
    }

    /// <summary>
    /// Updates repetition and offset scores and finds the most likely beat length and offset
    /// </summary>
    private void FindBeat()
    {
        currentSignal = Util.Smooth(currentSignal, 4);
        currentSignal = Util.Smooth(currentSignal, 4);
                
        Array.Clear(currentSignal, 0, 5);
        Array.Clear(currentSignal, currentSignal.Length - 5, 5);

        float[] stretchedSignal = new float[currentSignal.Length - 10];
        for (int i = 0; i < stretchedSignal.Length; i++)
        {
            stretchedSignal[i] = currentSignal[i + 5];
        }

        stretchedSignal = Util.StretchArray(stretchedSignal, 4);

        //score repetition
        for (int i = 0; i < repetitionScore.Length; i++)
            repetitionScore[i] = RepetitionScore(stretchedSignal, i);

        //normalize
        bestRepetition = 40;
        for (int i = 40; i < repetitionScore.Length - 5; i++)
        {
            if (repetitionScore[i] > repetitionScore[bestRepetition])
                bestRepetition = i;
        }
        float fr = repetitionScore[bestRepetition];
        for (int i = 0; i < repetitionScore.Length; i++)
        {
            repetitionScore[i] = repetitionScore[i] / fr;
        }

        //gap score
        for (int i = 45; i < gapScore.Length; i++)
        {
            gapScore[i] = GapScore(repetitionScore, i);                        
        }
        int bestGapScore = 45;
        for (int i = 45; i < gapScore.Length - 1; i++)
        {
            if (gapScore[i] > gapScore[bestGapScore])
                bestGapScore = i;
        }        

        //peak finding
        peaks.Clear();
        for (int i = 0; i < repetitionScore.Length - 5; i++)
        {
            if (i + 1 < repetitionScore.Length && i - 1 >= 0)
            {
                int t = i;
                while (t < 45)
                    t *= 2;
                while (t > gapScore.Length - 1)
                    t /= 2;

                if (gapScore[t / 2] > gapScore[t])
                    t /= 2;

                //this does two things:
                //1) if there is no easily discernible rhythm, all peaks will get ignored
                //2) if there is a rhythm, only peaks that are in line with a rhythm will get selected
                float w = repetitionScore[i] * Mathf.Max(0, gapScore[t]);
          
                float threshold = .425f;                                

                if (beatHistogram[currentBeatLength] < 7)
                    threshold = .25f;

                if (repetitionScore[i] > repetitionScore[i + 1] && repetitionScore[i] > repetitionScore[i - 1] && w > threshold) //.9f
                {
                    peaks.Add(i);
                }
            }
        }

        if(peaks.Count == 0)
        {
            if (repetitionScore[bestRepetition / 2] > .75f)
                peaks.Add(bestRepetition);
            else if (bestRepetition * 2 < repetitionScore.Length)            
                if (repetitionScore[bestRepetition * 2] > .75f)
                    peaks.Add(bestRepetition);
        }

        //if no peak is found and there is no beat length found add the peak with the biggest index possible based on gapScore
        if (peaks.Count == 0 && currentBeatLength <= 5)
        {
            int maxPeak = bestGapScore;

            while (maxPeak < 90)
                maxPeak *= 2;
            while (maxPeak > 90)
                maxPeak /= 2;

            peaks.Add(maxPeak);
        }

        peaks.Insert(0, 0);

        for (int i = 0; i < peaks.Count; i++)
        {
            if (i + 1 < peaks.Count)
            {
                int gap = peaks[i + 1] - peaks[i];

                if (gap > 3)
                {
                    //double or half gap size until they're within a range that's sensible
                    while (gap < 45) //45
                    {
                        gap *= 2;
                    }
                    while (gap > 90) //90
                    {
                        gap /= 2;
                    }
                    beatHistogram[gap]++;
                }
            }
        }

        //find best beat length
        currentBeatLength = 1;
        for (int i = 1; i < beatHistogram.Length; i++)
        {
            if (beatHistogram[i] > beatHistogram[currentBeatLength])
                currentBeatLength = i;
        }

        if (beatHistogram[currentBeatLength] > 15)
        {
            for (int i = 0; i < beatHistogram.Length; i++)
            {
                beatHistogram[i] = Mathf.Clamp(beatHistogram[i] - 7, 0, 7);
            }
        }

        //offset score
        for (int i = 0; i < offsetScore.Length; i++)
        {
            offsetScore[i] = OffsetScore(stretchedSignal, i, currentBeatLength);
        }

        //find best offset
        int offset = 0;
        for (int i = 1; i < offsetScore.Length - 1; i++)
        {
            if (offsetScore[i] > offsetScore[offset] && offsetScore[i] > offsetScore[i + 1] && offsetScore[i] > offsetScore[i - 1])
            {
                offset = i;
            }
        }

        int s = 0;
        if (offset < offsetScore.Length - 1)
        {
            offset = offset % currentBeatLength;
            s = offset;
            if (currentBeatLength > 0)
            {
                while (s < 0)
                    s += currentBeatLength;
            }
            s = currentBeatLength - s;
        }

        float d = s - syncOld;

        //push sync towards the right offset
        if (Mathf.Abs(d) > currentBeatLength / 2f)
        {
            if (d > 0)
                sync += -2;
            else
                sync += 2;
        }
        else
        {
            if (d > 2)
                sync += 2;
            if (d < -2)
                sync += -2;
        }
    }

    private float RepetitionScore(float[] signal, int offset)
    {
        float score = 0;

        for (int i = 0; i < signal.Length - offset; i++)
        {
            score += (signal[i] * signal[i + offset]);
        }

        return (score / (signal.Length - offset)) * 10;
    }

    private float GapScore(float[] signal, int offset)
    {
        float e = 0;
        int i = 1;
        int count = 0;
        while (offset * i < signal.Length)
        {
            e += signal[i * offset];
            i++;
            count++;
        }

        if (count == 0)
            return e;

        return e / count;       
    }

    private float OffsetScore(float[] signal, int offset, int gap)
    {
        float score = 0;
        for (int i = 0; i < signal.Length - offset; i++)
        {
            if ((i) % gap == 0)
                score += (signal[i + offset]);
        }

        score /= (signal.Length - offset) / gap;

        return score;
    }

    public void DrawDebugLines(int currentFrame)
    {
        for (int i = currentFrame; i < currentFrame + 300; i++)
        {
            if (_beats.ContainsKey(i))
            {
                Debug.DrawLine(new Vector3(i - currentFrame, 0, -1), new Vector3(i - currentFrame, 400, 1), Color.black);
            }
        }

        for (int i = 0; i < beatHistogram.Length; i++)
        {
            Vector3 sv = new Vector3(i * -1, 0, 0);
            Vector3 ev = new Vector3(i * -1, beatHistogram[i], 0);
            sv += Vector3.up * 100;
            ev += Vector3.up * 100;
            Debug.DrawLine(sv, ev, Color.blue);
        }

        for (int i = 0; i < signalBuffer.Length - 1; i++)
        {
            Vector3 start = new Vector3(i, signalBuffer[i], 0);
            Vector3 end = new Vector3((i + 1), signalBuffer[i + 1], 0);
            Debug.DrawLine(start, end, Color.black);
        }
        
        for (int i = 0; i < currentSignal.Length - 1; i++)
        {
            Vector3 start = new Vector3(i, currentSignal[i] - 200, 0);
            Vector3 end = new Vector3((i + 1), currentSignal[i + 1] - 200, 0);
            Debug.DrawLine(start, end);
        }

        for (int i = 0; i < repetitionScore.Length - 1; i++)
        {
            Vector3 start = new Vector3(i, repetitionScore[i] * 10 - 100, 0);
            Vector3 end = new Vector3((i + 1), repetitionScore[i + 1] * 10 - 100, 0);
            Debug.DrawLine(start, end, Color.gray);
        }

        for (int i = 0; i < gapScore.Length - 1; i++)
        {
            Vector3 start = new Vector3(i, gapScore[i] * 10 - 100, 0);
            Vector3 end = new Vector3((i + 1), gapScore[i + 1] * 10 - 100, 0);
            Debug.DrawLine(start, end, Color.black);
        }

        for (int i = 0; i < offsetScore.Length - 1; i++)
        {
            Vector3 start = new Vector3(i, offsetScore[i] - 150, 0);
            Vector3 end = new Vector3((i + 1), offsetScore[i + 1] - 150, 0);
            Debug.DrawLine(start, end, Color.red);
        }
        
        foreach (int p in peaks)
        {
            Debug.DrawLine(new Vector3(p, -100, 0), new Vector3(p, -50, 0));

            int t = p;

            if (t > 0)
            {
                while (t < 45)
                    t *= 2;
                while (t > 149)
                    t /= 2;
            }

            Debug.DrawLine(new Vector3(t, -100, 0), new Vector3(t, -90, 0), Color.black);
        }

        Debug.DrawLine(new Vector3(bestRepetition, -100, 0), new Vector3(bestRepetition, -70, 0), Color.yellow);
        
        int b = bestRepetition;

        if (b > 0)
        {
            while (b < 45)
                b *= 2;
            while (b > gapScore.Length - 1)
                b /= 2;
        }
        Debug.DrawLine(new Vector3(b, -100, 0), new Vector3(b, -80, 0), Color.yellow);        
    }

    /// <summary>
	/// The closest beat following index	
	/// </summary>
	/// <returns>The beat.</returns>
	/// <param name="index">Index.</param>
	public Beat NextBeat(int index)
    {
        if (_beats.Count == 0)
            return new Beat(0, 0, 0);

        int nextBeat = NextBeatIndex(index);

        return _beats[nextBeat];
    }

    /// <summary>
    /// The closest beat preceding index	
    /// </summary>
    /// <returns>The beat.</returns>
    /// <param name="index">Index.</param>
    public Beat PrevBeat(int index)
    {
        if (_beats.Count == 0)
            return new Beat(0, 0, 0);

        int prevBeat = PrevBeatIndex(index);

        return _beats[prevBeat];
    }

    /// <summary>
    /// The index of the closest beat following index	
    /// </summary>
    /// <returns>The beat index.</returns>
    /// <param name="index">Index.</param>
    public int NextBeatIndex(int index)
    {
        if (_beatIndices.Count == 0)
            return 0;

        int nextBeat = _beatIndices.BinarySearch(index);
        nextBeat = Mathf.Max(nextBeat, ~nextBeat);
        nextBeat = Mathf.Clamp(nextBeat, 0, _beatIndices.Count - 1);

        nextBeat = _beatIndices[nextBeat];
        return nextBeat;
    }

    /// <summary>
    /// The index of the closest beat preceding index	
    /// </summary>
    /// <returns>The beat index.</returns>
    /// <param name="index">Index.</param>
    public int PrevBeatIndex(int index)
    {
        if (_beatIndices.Count == 0)
            return 0;

        int prevBeat = _beatIndices.BinarySearch(index);
        prevBeat = Mathf.Max(prevBeat, ~prevBeat);
        prevBeat = Mathf.Clamp(prevBeat - 1, 0, _beatIndices.Count - 1);

        prevBeat = _beatIndices[prevBeat];
        return prevBeat;
    }

    /// <summary>
    /// A timer for the beat that is occuring at index.
    /// </summary>
    /// <returns>A value ranging from 0 to 1.</returns>
    /// <param name="index">Index.</param>
    public float BeatTimer(float index)
    {
        int nextBeat = NextBeatIndex((int)Mathf.Ceil(index));
        int prevBeat = PrevBeatIndex((int)Mathf.Ceil(index));

        int l = nextBeat - prevBeat;

        if (l == 0)
            return 0;

        return 1 - ((nextBeat - index) / l);
    }

    /// <summary>
    /// Does a beat occur at the specified index? 
    /// </summary>
    /// <returns><c>1</c> if whole beat; <c>2</c> if half beat; <c>4</c> if quarter beat.</returns>
    /// <param name="index">Index.</param>
    /// <param name="max">The minimum length for half and quarter beats.</param>
    public int IsBeat(int index, int min)
    {
        if (_beats.Count == 0)
            return 0;

        int nextBeat = NextBeatIndex((int)index);
        int prevBeat = PrevBeatIndex((int)index);

        float length = _beats[prevBeat].length;

        int r = nextBeat - prevBeat;

        int f = nextBeat - index;

        if (f == 0)
            return 1;
        if (length / 2 < min)
            return 0;
        if (f == r / 2)
            return 2;

        if (length / 4 < min)
            return 0;
        if (f == r / 4 || f == (r / 2) + (r / 4))
            return 4;

        return 0;
    }
}
