using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Analysis analyzes a song based on it's spectral magnitude at different moments in time.
/// Can be configured to analyze certain frequency ranges.
/// </summary>
[System.Serializable]
public class Analysis
{    
    public AnalysisData analysisData { get; private set; }

    /// <summary>
    /// list of onset indices 
    /// </summary>
    private List<int> onsetIndices;

    /// <summary>
    /// List of all detected onsets.
    /// </summary>
    private Dictionary<int, Onset> _onsets;

    /// <summary>
    /// Magnitude or loudness.
    /// </summary>
    private List<float> _magnitude;
    /// <summary>
    /// Smoothed magnitude
    /// </summary>
    private List<float> _magnitudeSmooth;
    /// <summary>
    /// Flux or difference between current and previous frame.
    /// </summary>
    private List<float> _flux;
    /// <summary>
    /// Average of smoothed magnitude interpolated peaks and troughs. 
    /// </summary>
    private List<float> _magnitudeAvg;

    private int t1 = 0;
    private int t2 = 0;

    private int p1 = 0;
    private int p2 = 0;

    /// <summary>
    /// The threshold multiplier for finetuning the peak-picking threshold.
    /// </summary>
    private static float thresholdMultiplier = 1.9f;
    /// <summary>
    /// The size of the threshold window. How much of the neighboring data is taken into account
    /// when calculating the peak-picking threshold.
    /// </summary>
    private static int thresholdWindowSize = 6;
        
    private int start;
    private int end;

    /// <summary>
    /// The total number of frames in the song.
    /// </summary>	
    private int totalFrames;

    public string name { get; private set; }

    public Analysis(int start, int end, string name)
    {
        this.name = name;
        this.start = start;
        this.end = end;

        _magnitude = new List<float>();
        _flux = new List<float>();
        _magnitudeSmooth = new List<float>();
        _magnitudeAvg = new List<float>();
        _onsets = new Dictionary<int, Onset>();

        onsetIndices = new List<int>();

        analysisData = new AnalysisData(name, _magnitude, _flux, _magnitudeSmooth, _magnitudeAvg, _onsets);
    }
    
    /// <summary>
    /// Initialize the analysis for a new song.
    /// </summary>
    /// <param name='totalFrames'>
    /// Length of the new song.
    /// </param>
    public void Init(int totalFrames)
    {
        this.totalFrames = totalFrames;

        onsetIndices.Clear();

        _magnitude.Clear();
        _flux.Clear();
        _magnitudeSmooth.Clear();
        _magnitudeAvg.Clear();
        _onsets.Clear();

        _magnitude.AddRange(new float[totalFrames]);
        _flux.AddRange(new float[totalFrames]);
        _magnitudeSmooth.AddRange(new float[totalFrames]);
        _magnitudeAvg.AddRange(new float[totalFrames]);

        _magnitude.TrimExcess();
        _flux.TrimExcess();
        _magnitudeSmooth.TrimExcess();
        _magnitudeAvg.TrimExcess();

        _magnitude[4] = 4;
        
        t1 = 0;
        t2 = 0;

        p1 = 0;
        p2 = 0;

        int spectrumSize = (RhythmTool.fftWindowSize - 2) / 2;
        if (end < start || start < 0 || end < 0 || start >= spectrumSize || end > spectrumSize)
            Debug.LogError("Invalid range for analysis " + name + ". Range must be within Spectrum (fftWindowSize/2 - 1) and start cannot come after end.");
    }

    /// <summary>
    /// Initialize the analysis with existing data.
    /// </summary>
    /// <param name="data"></param>
    public void Init(AnalysisData data)
    {
        onsetIndices.Clear();

        _magnitude.Clear();
        _flux.Clear();
        _magnitudeSmooth.Clear();
        _magnitudeAvg.Clear();
        _onsets.Clear();

        _magnitude.AddRange(data.magnitude);
        _flux.AddRange(data.flux);
        _magnitudeSmooth.AddRange(data.magnitudeSmooth);
        _magnitudeAvg.AddRange(data.magnitudeAvg);

        _magnitude.TrimExcess();
        _flux.TrimExcess();
        _magnitudeSmooth.TrimExcess();
        _magnitudeAvg.TrimExcess();

        foreach(KeyValuePair<int, Onset> onset in data.onsets)
        {
            _onsets.Add(onset.Key, onset.Value);
        }
    }

    /// <summary>
    /// Returns an onset. Onset will be 0 if there is none detected for this index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Onset GetOnset(int index)
    {
        Onset o;

        _onsets.TryGetValue(index, out o);

        return o;
    }
    
    /// <summary>
    /// Analyze the specified spectrum based on frequency ranges and previous spectra.
    /// </summary>
    /// <param name='spectrum'>
    /// A spectrum.
    /// </param>
    /// <param name='index'>
    /// The index of this spectrum.
    /// </param>
    public void Analyze(float[] spectrum, int index)
    {
        if (index == totalFrames - 1)
        {
            t2 = index;
            p2 = index;
        }

        //Offset for each different step in the analysis.
        int offset = 0;
        
        //get the magnitude or loudness
        _magnitude[index] = Util.Sum(spectrum, start, end);
        _magnitudeSmooth[index] = _magnitude[index];

        //start smooting magnitudeSmooth
        offset = Mathf.Max(index - 10, 0);
        Smooth(_magnitudeSmooth, offset, 10);
        
        //get the difference between this and previous magnitude
        _flux[offset] = _magnitude[offset] - _magnitude[Mathf.Max(offset - 1, 0)];

        offset = Mathf.Max(index - 20, 0);
        Smooth(_magnitudeSmooth, offset, 10);

        //if the difference between this and previous magnitude is big enough, it's an onset
        float t = Threshold(_flux, offset, thresholdMultiplier, thresholdWindowSize);
        if (_flux[offset] > t && _flux[offset] > _flux[offset + 1] && _flux[offset] > _flux[offset - 1])
        {
            Onset o = new Onset(offset,_flux[offset],0);
            _onsets.Add(offset, o);
            onsetIndices.Add(offset);
        }

        offset = Mathf.Max(index - 30, 0);
        Smooth(_magnitudeSmooth, offset, 10);

        //rank onset if it's there
        offset = Mathf.Max(index - 100, 0);
        Rank(offset, 50);
        
        //some more smooting
        offset = Mathf.Max(index - 40, 1);
        Smooth(_magnitudeSmooth, offset, 10);

        offset = Mathf.Max(index - 50, 1);
        Smooth(_magnitudeSmooth, offset, 10);

        offset = Mathf.Max(index - 60, 1);
        Smooth(_magnitudeSmooth, offset, 10);

        offset = Mathf.Max(index - 70, 1);
        Smooth(_magnitudeSmooth, offset, 10);

        offset = Mathf.Max(index - 80, 1);
        Smooth(_magnitudeSmooth, offset, 10);

        offset = Mathf.Max(index - 90, 1);
        Smooth(_magnitudeSmooth, offset, 10);
        
        //interpolate between peaks and troughs and get the average 
        offset = Mathf.Max(index - 100, 1);

        if (_magnitudeSmooth[offset] < _magnitudeSmooth[offset - 1] && _magnitudeSmooth[offset] < _magnitudeSmooth[offset + 1])
        {
            t1 = t2;
            t2 = offset;
        }

        if (t1 != t2)
        {           
            if(t2<p2)
            {
                int nt = t2 - t1;
                int np = p2 - p1;

                float ft = (_magnitudeSmooth[t2] - _magnitudeSmooth[t1])/nt;
                float fp = (_magnitudeSmooth[p2] - _magnitudeSmooth[p1])/np;

                for (int i = p1; i < t2; i++)
                {
                    int ti = i - t1;
                    int pi = i - p1;

                    _magnitudeAvg[i] = (_magnitudeSmooth[t1] + (ft*ti))+ (_magnitudeSmooth[p1] + (fp * pi));
                    _magnitudeAvg[i] *= .5f;
                }
            }
        }

        if (_magnitudeSmooth[offset] > _magnitudeSmooth[offset - 1] && _magnitudeSmooth[offset] > _magnitudeSmooth[offset + 1])
        {
            p1 = p2;
            p2 = offset;
        }

        if (p1 != p2)
        {
            if (p2 < t2)
            {
                int nt = t2 - t1;
                int np = p2 - p1;

                float ft = (_magnitudeSmooth[t2] - _magnitudeSmooth[t1]) / nt;
                float fp = (_magnitudeSmooth[p2] - _magnitudeSmooth[p1]) / np;

                for (int i = t1; i < p2; i++)
                {
                    int ti = i - t1;
                    int pi = i - p1;

                    _magnitudeAvg[i] = (_magnitudeSmooth[t1] + (ft * ti)) + (_magnitudeSmooth[p1] + (fp * pi));
                    _magnitudeAvg[i] *= .5f;
                }
            }
        }
    }

    /// <summary>
    /// Draws the different results in a graph.
    /// </summary>
    /// <param name='index'>
    /// The index from where to start.
    /// </param>
    /// <param name='h'>
    /// The 
    /// </param>
    public void DrawDebugLines(int index, int h)
    {
        for (int i = 0; i < 299; i++)
        {
            if (i + 1 + index > totalFrames - 1)
                break;
            Vector3 s = new Vector3(i, _magnitude[i + index] + h * 100, 0);
            Vector3 e = new Vector3(i + 1, _magnitude[i + 1 + index] + h * 100, 0);
            Debug.DrawLine(s, e, Color.red);

            s = new Vector3(i, _magnitudeSmooth[i + index] + h * 100, 0);
            e = new Vector3(i + 1, _magnitudeSmooth[i + 1 + index] + h * 100, 0);
            Debug.DrawLine(s, e, Color.red);            

            s = new Vector3(i, _magnitudeAvg[i + index] + h * 100, 0);
            e = new Vector3(i + 1, _magnitudeAvg[i + 1 + index] + h * 100, 0);
            Debug.DrawLine(s, e, Color.black);

            s = new Vector3(i, _flux[i + index] + h * 100, 0);
            e = new Vector3(i + 1, _flux[i + 1 + index] + h * 100, 0);
            Debug.DrawLine(s, e, Color.blue);

            if (_onsets.ContainsKey(i + index))
            {
                Onset o = _onsets[i + index];
                                
                s = new Vector3(i, h * 100, -1);
                e = new Vector3(i, o.strength + h * 100, -1);
                Debug.DrawLine(s, e, Color.green);

                s = new Vector3(i, h * 100, 0);
                e = new Vector3(i, -o.rank + h * 100, 0);
                Debug.DrawLine(s, e, Color.white);                
            }
        }
    }

    private void Rank(int index, int windowSize)
    {
        if (!_onsets.ContainsKey(index))
            return;
        
        int indexOf = onsetIndices.IndexOf(index);

        int rank = 0;

        for(int i = 5; i>0; i--)
        {            
            if(indexOf-i>0&&indexOf+i<onsetIndices.Count)
            {
                float c = _flux[index];
                float p = _flux[onsetIndices[indexOf - i]];
                float n = _flux[onsetIndices[indexOf + i]];

                if(c>p && c>n)
                {
                    rank = 6-i;
                }

                if (onsetIndices[indexOf - i] < index - windowSize / 2 && onsetIndices[indexOf + i] > index + windowSize / 2)
                    rank = 6-i;
            }
        }
        _onsets[index].rank = rank;
    }
       
    private float Threshold(List<float> input, int index, float multiplier, int windowSize)
    {
        int start = Mathf.Max(0, index - windowSize);
        int end = Mathf.Min(input.Count - 1, index + windowSize);
        float mean = 0;
        for (int i = start; i <= end; i++)
            mean += Mathf.Abs(input[i]);
        mean /= (end - start);

        return Mathf.Clamp(mean * multiplier, 3, 70);
    }

    private void Smooth(List<float> input, int index, int windowSize)
    {
        float average = 0;
        for (int i = index - (windowSize / 2); i < index + (windowSize / 2); i++)
        {
            if (i > 0 && i < totalFrames)
                average += input[i];
        }
        input[index] = average / windowSize;
    }    
}
