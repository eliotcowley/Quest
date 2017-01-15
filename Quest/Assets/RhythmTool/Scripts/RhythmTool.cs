using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;

/// <summary> 
/// RhythmTool.
/// </summary>
[System.Serializable]
[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Audio/RhythmTool")]
public class RhythmTool : MonoBehaviour
{
    /// <summary>
    /// Number of samples that fft is to be performed on. Must be a power of 2.
    /// </summary>
    public static int fftWindowSize { get { return 4096; } }

    /// <summary>
    /// How much the fft window moves for each frame.
    /// </summary>
    public static int frameSpacing { get { return 1470; } }

    /// <summary>
    /// beatTracker is fed data one frame at a time and tracks and stores beats
    /// </summary>
    private BeatTracker beatTracker;
    /// <summary>
    /// Dictionary with beat indices as keys and Beats as values
    /// </summary>
    public ReadOnlyDictionary<int, Beat> beats { get { return beatTracker.beats; } }

    /// <summary>
    /// segmenter looks at large changes in songs and stores them
    /// </summary>
    private Segmenter segmenter;
    /// <summary>
    /// Dictionary with change indices as keys and changes as values
    /// </summary>
    public ReadOnlyDictionary<int, float> changes { get { return segmenter.changes; } }

    /// <summary>
    /// Stores the coroutine that is currently analyzing
    /// </summary>
    private Coroutine analyzeRoutine = null;
    /// <summary>
    /// Stores the coroutine that is waiting for analyzeRoutine to end
    /// </summary>
    private Coroutine queueRoutine = null;

    [SerializeField]
    private bool _preCalculate = false;

    /// <summary>
    /// Set to "true" if you want the analyzer to pre calculate everything.
    /// Pre calculation allows for additional analysis, but it takes some time.
    /// </summary>
    public bool preCalculate
    {
        get { return _preCalculate; }
        set { if (analysisDone ^ !songLoaded) _preCalculate = value; }
    }

    /// <summary>
    /// Calculate tempo and do beat tracking 
    /// </summary>
    [SerializeField]
    private bool _calculateTempo = true;

    public bool calculateTempo
    {
        get { return _calculateTempo; }
        set { if (analysisDone ^ !songLoaded) _calculateTempo = value; }
    }

    [SerializeField]
    private bool _storeAnalyses = false;

    /// <summary>
    /// Store and load analysis results.
    /// </summary>
    public bool storeAnalyses
    {
        get { return _storeAnalyses; }
        set { if (analysisDone ^ !songLoaded) _storeAnalyses = value; }
    }

    [SerializeField]
    private int _lead = 300;

    /// <summary>
    /// How much of a lead (in frames) the analysis has on the song. 
    /// Can be changed at runtime. See "preCalculate" for pre calculating all data.
    /// </summary>
    public int lead
    {
        get { return _lead; }
        set { _lead = Mathf.Max(_lead, 300); }
    }

    /// <summary>
    /// samples from the audioclip to use for FFT
    /// </summary>
    private float[] samples = new float[fftWindowSize];

    /// <summary>
    /// The total number of frames for this song.
    /// </summary>
    public int totalFrames { get; private set; }

    /// <summary>
    /// The frame corresponding with the current time of the song.
    /// Important for synchronizing the data with the song.
    /// </summary>
    public int currentFrame { get; private set; }

    /// <summary>
    /// The time in between the current frame and the next.
    /// </summary>
    /// <value>
    /// The interpolation.
    /// </value>
    public float interpolation { get; private set; }

    /// <summary>
    /// The last frame that has been used to pass data
    /// </summary>
    private int lastDataFrame = 0;

    /// <summary>
    /// The last frame that has been analyzed.
    /// </summary>
    public int lastFrame { get { return _lastFrame; } }

    //serialized private field so progress can be trakced when preCalculate is enabled
    [SerializeField]
    private int _lastFrame = 0;

    /// <summary>
    /// The total amount of samples of the music file.
    /// </summary>
    private int totalSamples;

    /// <summary>
    /// The the index of the current sample of the song.
    /// </summary>
    private int sampleIndex;

    /// <summary>
    /// Is the analsis done?
    /// </summary>
    public bool analysisDone { get; private set; }

    private List<Analysis> analyses;

    public AnalysisData low { get { return _low.analysisData; } }
    private Analysis _low;

    public AnalysisData mid { get { return _mid.analysisData; } }
    private Analysis _mid;

    public AnalysisData high { get { return _high.analysisData; } }
    private Analysis _high;

    public AnalysisData all { get { return _all.analysisData; } }
    private Analysis _all;

    /// <summary>
    /// Most probable BPM currently. 
    /// </summary>
    public float bpm { get { return _bpm; } }

    [SerializeField]
    private float _bpm = 0;

    /// <summary>
    /// The length of the beat in frames that is currently being played.
    /// </summary>
    public float beatLength { get { return _beatLength; } }

    [SerializeField]
    private float _beatLength;

    private AudioSource audioSource;

    private float _frameLength = .033333f;

    /// <summary>
    /// The length of a frame in seconds.
    /// </summary>
    public float frameLength { get { return _frameLength; } }

    /// <summary>
    /// Is a song currently loaded and ready to play?
    /// </summary>
    public bool songLoaded { get; private set; }

    public float volume
    {
        get { return audioSource.volume; }
        set { audioSource.volume = value; }
    }

    public float pitch
    {
        get { return audioSource.pitch; }
        set { audioSource.pitch = value; }
    }

    public bool isPlaying
    {
        get { return audioSource.isPlaying; }
    }

    void Init()
    {
        //default analyses.
        analyses = new List<Analysis>();

        _low = new Analysis(0, 30, "low");
        analyses.Add(_low);
        _mid = new Analysis(30, 350, "mid");
        analyses.Add(_mid);
        _high = new Analysis(370, 900, "high");
        analyses.Add(_high);
        _all = new Analysis(0, 350, "all");//560 //350
        analyses.Add(_all);

        beatTracker = new BeatTracker();
        segmenter = new Segmenter(all);

        songLoaded = false;
    }

    void Awake()
    {
        Init();
        audioSource = GetComponent<AudioSource>();
        RhythmEventProvider.OnEventProviderEnabled += OnEventProviderEnabled;
    }

    private void OnEventProviderEnabled(RhythmEventProvider r)
    {
        if (songLoaded)
        {
            InitializeEventProvider(r);
        }
    }

    private void InitializeEventProvider(RhythmEventProvider r)
    {
        r.offset = 0;

        if (audioSource != null)
        {
            r.onNewSong.Invoke(audioSource.clip.name, totalFrames);
        }

        r.totalFrames = totalFrames;
    }

    private void InitializeEventProviders()
    {
        foreach (RhythmEventProvider r in RhythmEventProvider.eventProviders)
        {
            InitializeEventProvider(r);
        }
    }

    /// <summary>
    /// Use an AudioClip as the new song. (re) initializes analyses and variables.
    /// </summary>
    /// <param name='musicPath'>
    /// Audioclip of the song.
    /// </param>
    public void NewSong(AudioClip audioClip)
    {
        //if another song is queued, stop waiting and use this song instead
        if (queueRoutine != null)
            StopCoroutine(queueRoutine);

        queueRoutine = StartCoroutine(QueueNewSong(audioClip));
    }

    /// <summary>
    /// Queue a song to wait for analyzeRoutine to stop
    /// </summary>
    /// <param name="audioClip"></param>
    /// <returns></returns>
    IEnumerator QueueNewSong(AudioClip audioClip)
    {
        yield return analyzeRoutine;

        queueRoutine = null;

        LoadNewSong(audioClip);
    }

    private void LoadNewSong(AudioClip audioClip)
    {
        audioSource.Stop();
        audioSource.clip = audioClip;

        totalSamples = audioSource.clip.samples;
        totalSamples -= totalSamples % frameSpacing;
        totalFrames = totalSamples / frameSpacing;
        _frameLength = 1 / ((float)audioSource.clip.frequency / (float)frameSpacing);

        foreach (Analysis s in analyses)
        {
            s.Init(totalFrames);
        }

        beatTracker.Init(frameLength);
        segmenter.Init();

        analysisDone = false;

        currentFrame = 0;
        _lastFrame = 0;
        lastDataFrame = 0;

        songLoaded = false;

        if (!_preCalculate)
        {
            _storeAnalyses = false;
            analyzeRoutine = StartCoroutine(AsyncAnalyze(_lead + 300));
        }
        else
        {
            _lead = 300;
            if (_storeAnalyses)
            {
                if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + audioSource.clip.name + ".rthm"))
                {
                    SongData songData = SongData.Deserialize(audioSource.clip.name);

                    if (songData.length == totalFrames)
                    {
                        _lastFrame = totalFrames;
                        analysisDone = true;

                        foreach (AnalysisData data in songData.analyses)
                        {
                            foreach (Analysis a in analyses)
                            {
                                if (a.name == data.name)
                                {
                                    a.Init(data);
                                }
                            }
                        }

                        beatTracker.Init(songData);
                        segmenter.Init(songData);

                        songLoaded = true;

                        InitializeEventProviders();
                        gameObject.SendMessage("OnReadyToPlay", SendMessageOptions.DontRequireReceiver);

                        return;
                    }
                }
            }
            analyzeRoutine = StartCoroutine(AsyncAnalyze(totalFrames));
        }
    }

    /// <summary>
    /// Gets called when the analysis is completed. 
    /// </summary>
    private void EndOfAnalysis()
    {
        //fill beats till end
        if (_calculateTempo)
            beatTracker.FillEnd();

        if (_preCalculate)
        {
            //Do post-analysis here.

            if (_storeAnalyses)
            {
                List<AnalysisData> datas = new List<AnalysisData>();

                foreach (Analysis a in analyses)
                    datas.Add(a.analysisData);

                SongData songData = new SongData(datas, audioSource.clip.name, totalFrames, beatTracker, segmenter);
                songData.Serialize();
            }
        }
        analysisDone = true;
    }

    private void EndOfSong()
    {
        Stop();
        gameObject.SendMessage("OnEndOfSong", SendMessageOptions.DontRequireReceiver);
    }

    void Update()
    {
        if (!songLoaded)
            return;

        if (audioSource.clip == null)
            return;

        sampleIndex = audioSource.timeSamples;
        float time = (float)sampleIndex / (float)totalSamples;
        currentFrame = (int)(time * totalFrames);

        if (currentFrame >= totalFrames - 1)
        {
            EndOfSong();
            return;
        }

        interpolation = time * (float)totalFrames;
        interpolation = interpolation - (float)currentFrame;

        Beat nextBeat = NextBeat(currentFrame);
        _beatLength = nextBeat.length;
        _bpm = nextBeat.bpm;

        Analyze();
        PassData();
    }

    private void Analyze()
    {
        if (analysisDone)
            return;

        if (_preCalculate)
            _lead += 300;

        _lead = Mathf.Max(300, _lead);

        for (int i = _lastFrame + 1; i < currentFrame + _lead + 300; i++)
        {
            if (i >= totalFrames)
            {
                EndOfAnalysis();
                break;
            }

            audioSource.clip.GetData(samples, Mathf.Max((i * frameSpacing) - (fftWindowSize / 2), 0));

            float[] spectrum = Util.GetSpectrum(samples);

            foreach (Analysis s in analyses)
            {
                s.Analyze(spectrum, i);
            }

            if (_calculateTempo)
            {
                if (i - 10 >= 0)
                {
                    float flux = all.flux[i - 10];
                    beatTracker.TrackBeat(flux);
                }
            }

            segmenter.DetectChanges(i - 350);

            _lastFrame = i;
        }
    }

    private IEnumerator AsyncAnalyze(int frames)
    {
        frames = Mathf.Clamp(frames, 0, totalFrames);
        //AudioClip.GetData only works on the main thread
        float[] s = new float[frames * frameSpacing * 2];
        audioSource.clip.GetData(s, 0);

        Thread analyzeThread = new Thread(BackGroundAnalyze);
        analyzeThread.Start(s);

        while (analyzeThread.IsAlive)
        {
            yield return null;
        }

        //if a new song is queued at this point, stop
        if (queueRoutine != null)
        {
            analyzeRoutine = null;
            yield break;
        }

        _lastFrame = frames - 1;

        if (calculateTempo)
            beatTracker.FillStart();

        if (lastFrame > totalFrames - 2)
            EndOfAnalysis();

        songLoaded = true;

        analyzeRoutine = null;

        InitializeEventProviders();
        gameObject.SendMessage("OnReadyToPlay", SendMessageOptions.DontRequireReceiver);
    }

    private void BackGroundAnalyze(object o)
    {
        float[] s = (float[])o;

        int count = s.Length / frameSpacing / 2;

        for (int i = 0; i < count; i++)
        {
            for (int ii = 0; ii < samples.Length; ii++)
            {
                samples[ii] = s[Mathf.Max((i * frameSpacing * 2) - (fftWindowSize / 2) * 2, 0) + ii];
            }

            float[] spectrum = Util.GetSpectrum(samples);

            foreach (Analysis a in analyses)
            {
                a.Analyze(spectrum, i);
            }

            if (_calculateTempo)
            {
                if (i - 10 >= 0)
                {
                    float flux = all.flux[i - 10];
                    beatTracker.TrackBeat(flux);
                }
            }

            segmenter.DetectChanges(i - 350);

            _lastFrame = i;

            //stop if another song was queued
            if (queueRoutine != null)
                break;
        }
    }

    /// <summary>
    /// pass data and events to all RhythmEventProviders
    /// </summary>
    private void PassData()
    {
        foreach (RhythmEventProvider r in RhythmEventProvider.eventProviders)
        {
            r.targetOffset = Mathf.Clamp(r.targetOffset, 0, _lead - 25);
            r.currentFrame = currentFrame;
            r.interpolation = interpolation;

            if (r.offset != r.targetOffset)
            {
                //if this SongDataProvider's offset has increased, send all events that happened in between
                while (r.offset < r.targetOffset)
                {
                    PassSubBeat(r, currentFrame + r.offset + interpolation);
                    PassEvents(r, currentFrame + r.offset);

                    r.offset++;
                }

                r.offset = Mathf.Min(r.offset, r.targetOffset);
            }
            else
                PassSubBeat(r, currentFrame + r.offset + interpolation);

            if (r.timingUpdate.listenerCount != 0)
                r.timingUpdate.Invoke(currentFrame + r.offset, interpolation, NextBeat(currentFrame + r.offset).length, BeatTimer(currentFrame + r.offset + interpolation));
        }

        //if (currentFrame < lastDataFrame)
        //    lastDataFrame = currentFrame;

        //iterate over any frames that passed between this update and the previous update
        for (int i = lastDataFrame + 1; i < currentFrame + 1; i++)
        {
            foreach (RhythmEventProvider r in RhythmEventProvider.eventProviders)
            {
                PassEvents(r, i + r.offset);
                r.offset = r.targetOffset;
            }

            lastDataFrame = i;
        }
    }

    private void PassSubBeat(RhythmEventProvider r, float index)
    {
        float beatTime = BeatTimer(index);
        Beat prevBeat = PrevBeat(Mathf.CeilToInt(index));

        if (r.lastBeatTime > beatTime)
            r.onSubBeat.Invoke(prevBeat, 0);
        if (r.lastBeatTime < .5f && beatTime >= .5f)
            r.onSubBeat.Invoke(prevBeat, 2);
        if (r.lastBeatTime < .25f && beatTime >= .25f)
            r.onSubBeat.Invoke(prevBeat, 1);
        if (r.lastBeatTime < .75f && beatTime >= .75f)
            r.onSubBeat.Invoke(prevBeat, 3);

        r.lastBeatTime = beatTime;
    }

    private void PassEvents(RhythmEventProvider r, int index)
    {
        r.onFrameChanged.Invoke(index, lastFrame);

        if (r.onOnset.listenerCount != 0)
        {
            Onset l = _low.GetOnset(index);
            Onset m = _mid.GetOnset(index);
            Onset h = _high.GetOnset(index);
            Onset a = _all.GetOnset(index);

            if (l > 0)
                r.onOnset.Invoke(OnsetType.Low, l);
            if (m > 0)
                r.onOnset.Invoke(OnsetType.Mid, m);
            if (h > 0)
                r.onOnset.Invoke(OnsetType.High, h);
            if (a > 0)
                r.onOnset.Invoke(OnsetType.All, a);
        }

        if (r.onBeat.listenerCount != 0)
        {
            if (IsBeat(index) == 1)
            {
                Beat beat = NextBeat(index);
                r.onBeat.Invoke(beat);
            }
        }

        if (r.onChange.listenerCount != 0)
        {
            if (IsChange(index))
            {
                r.onChange.Invoke(index, NextChange(index));
            }
        }
    }

    /// <summary>
    /// Plays the song from the start
    /// </summary>
    public void Play()
    {
        if (songLoaded)
        {
            lastDataFrame = currentFrame;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Stops playing the song
    /// </summary>
    public void Stop()
    {
        if (songLoaded)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Pauses the song
    /// </summary>
    public void Pause()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// Unpauses the song
    /// </summary>
    public void UnPause()
    {
        audioSource.UnPause();
    }

    /// <summary>
    /// returns the time (in seconds) corresponding with the specified frame.
    /// </summary>
    /// <returns>
    /// time (in seconds).
    /// </returns>
    /// <param name='index'>
    /// Index of frame.
    /// </param>
    public float TimeSeconds(int index)
    {
        return (index * _frameLength);
    }

    /// <summary>
    /// returns the time (in seconds) corresponding with the current frame.
    /// </summary>
    /// <returns>
    /// time (in seconds)
    /// </returns>
    public float TimeSeconds()
    {
        return TimeSeconds(currentFrame);
    }

    public void GetSpectrum(float[] samples, int channel, FFTWindow window)
    {
        audioSource.GetSpectrumData(samples, channel, window);
    }

    /// <summary>
    /// Add a custom analysis.
    /// </summary>
    /// <param name='s'>
    /// Start of frequency range.
    /// </param>
    /// <param name='e'>
    /// End of frequency range.
    /// </param>
    /// </param>
    /// <param name='name'>
    /// name of this analysis in order to retrieve it when loading analysis results from a previous session.
    /// </param>
    public AnalysisData AddAnalysis(int s, int e, string name)
    {
        foreach (Analysis an in analyses)
        {
            if (an.name == name)
            {
                Debug.LogWarning("Analysis with name " + name + " already exists");
                return null;
            }
        }

        Analysis a = new Analysis(s, e, name);
        a.Init(totalFrames);
        analyses.Add(a);

        return a.analysisData;
    }

    /// <summary>
    /// Returns AnalysisData of the Analysis with name. Null if Analysis was not found.
    /// </summary>
    /// <param name='name'>
    /// Name.
    /// </param>
    public AnalysisData GetAnalysis(string name)
    {
        foreach (Analysis a in analyses)
        {
            if (a.name == name)
                return a.analysisData;
        }

        Debug.LogWarning("Analysis with name " + name + " was not found");

        return null;
    }

    /// <summary>
    /// Draws graphs representing the data.  
    /// </summary>
    public void DrawDebugLines()
    {
        if (!Application.isEditor)
            return;

        if (!songLoaded)
            return;

        for (int i = 0; i < analyses.Count; i++)
        {
            analyses[i].DrawDebugLines(currentFrame, i);
        }

        if (_calculateTempo)
        {
            beatTracker.DrawDebugLines(currentFrame);
        }
    }

    /// <summary>
    /// The closest beat following index	
    /// </summary>
    /// <returns>The beat.</returns>
    /// <param name="index">Index.</param>
    public Beat NextBeat(int index)
    {
        return beatTracker.NextBeat(index);
    }

    /// <summary>
    /// The closest beat preceding index	
    /// </summary>
    /// <returns>The beat.</returns>
    /// <param name="index">Index.</param>
    public Beat PrevBeat(int index)
    {
        return beatTracker.PrevBeat(index);
    }

    /// <summary>
    /// The index of the closest beat following index	
    /// </summary>
    /// <returns>The beat index.</returns>
    /// <param name="index">Index.</param>
    public int NextBeatIndex(int index)
    {
        return beatTracker.NextBeatIndex(index);
    }

    /// <summary>
    /// The index of the closest beat following currentFrame	
    /// </summary>
    /// <returns>The beat index.</returns>
    /// <param name="index">Index.</param>
    public int NextBeatIndex()
    {
        return NextBeatIndex(currentFrame);
    }

    /// <summary>
    /// The index of the closest beat preceding currentFrame	
    /// </summary>
    /// <returns>The beat index.</returns>
    /// <param name="index">Index.</param>
    public int PrevBeatIndex()
    {
        return PrevBeatIndex(currentFrame);
    }

    /// <summary>
    /// The index of the closest beat preceding index	
    /// </summary>
    /// <returns>The beat index.</returns>
    /// <param name="index">Index.</param>
    public int PrevBeatIndex(int index)
    {
        return beatTracker.PrevBeatIndex(index);
    }

    /// <summary>
    /// A timer for the beat that is occuring at index.
    /// </summary>
    /// <returns>A value ranging from 0 to 1.</returns>
    /// <param name="index">Index.</param>
    public float BeatTimer(float index)
    {
        return beatTracker.BeatTimer(index);
    }

    /// <summary>
    /// A timer for the beat that is currently occuring.
    /// </summary>
    /// <returns>A value ranging from 0 to 1.</returns>
    public float BeatTimer()
    {
        return BeatTimer(currentFrame + interpolation);
    }

    /// <summary>
    /// Does a beat occur at the specified index? 
    /// </summary>
    /// <returns><c>1</c> if whole beat; <c>2</c> if half beat; <c>4</c> if quarter beat.</returns>
    /// <param name="index">Index.</param>
    /// <param name="max">The minimum length for half and quarter beats.</param>
    public int IsBeat(int index, int min)
    {
        return beatTracker.IsBeat(index, min);
    }

    /// <summary>
    /// Does a beat occur at the specified index? 
    /// </summary>
    /// <returns><c>1</c> if whole beat; <c>2</c> if half beat; <c>4</c> if quarter beat.</returns>
    /// <param name="index">Index.</param>
    public int IsBeat(int index)
    {
        return IsBeat(index, 0);
    }

    /// <summary>
    /// Does a change occur at the specified index?
    /// </summary>
    /// <param name="index"></param>
    /// <returns><c>true</c> if a change occurs</returns>
    public bool IsChange(int index)
    {
        return segmenter.IsChange(index);
    }

    /// <summary>
    /// The index of the closest change preceding index	
    /// </summary>
    /// <returns>The change index.</returns>
    /// <param name="index">Index.</param>
    public int PrevChangeIndex(int index)
    {
        return segmenter.PrevChangeIndex(index);
    }

    /// <summary>
    /// The index of the closest change following index	
    /// </summary>
    /// <returns>The change index.</returns>
    /// <param name="index">Index.</param>
    public int NextChangeIndex(int index)
    {
        return segmenter.NextChangeIndex(index);
    }

    /// <summary>
	/// The closest change preceding index	
	/// </summary>
	/// <returns>positive if change is greater than previous value, negative if less than previous value</returns>
	/// <param name="index">Index.</param>
	public float PrevChange(int index)
    {
        return segmenter.PrevChange(index);
    }

    /// <summary>
	/// The closest change preceding index	
	/// </summary>
	/// <returns>positive if change is greater than previous value, negative if less than previous value</returns>
	/// <param name="index">Index.</param>
	public float NextChange(int index)
    {
        return segmenter.NextChange(index);
    }
}