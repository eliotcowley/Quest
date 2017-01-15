using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using UnityEngine.Events;

public enum OnsetType { Low, Mid, High, All }

/// <summary>
/// Component that provides UnityEvents. Can be used anywhere and RhythmTool will find all RhythmEventProviders and call it's events when needed.
/// </summary>
[AddComponentMenu("Audio/RhythmEventProvider")]
public class RhythmEventProvider : MonoBehaviour
{
    private static ReadOnlyCollection<RhythmEventProvider> _eventProviders;
    public static ReadOnlyCollection<RhythmEventProvider> eventProviders
    {
        get
        {
            if (_eventProviders == null)
                _eventProviders = eventProviderList.AsReadOnly();
            return _eventProviders;
        }
    }

    [Tooltip("How many frames in advance events will be called")]
    public int targetOffset;

    /// <summary>
    /// The offset for this SongDataProvider. Increase to call events in advance.
    /// Use targetOffset to not skip any events when changing offset.
    /// </summary>
    [HideInInspector]
    public int offset;

    [HideInInspector]
    public float lastBeatTime;

    /// <summary>
    /// current frame without offset applied
    /// </summary>
    public int currentFrame = 0;
    /// <summary>
    /// current interpolation between current frame and next frame
    /// </summary>
    public float interpolation = 0;

    public int totalFrames = 0;

    [System.Serializable]
    public class OnBeat : UnityEvent<Beat>
    {
        private int _listenerCount = 0;
        public int listenerCount { get { return _listenerCount + GetPersistentEventCount(); } }

        /// <summary>
        /// beat
        /// </summary>
        /// <param name="call"></param>
        new public void AddListener(UnityAction<Beat> call)
        {
            _listenerCount++;
            base.AddListener(call);
        }

        new public void RemoveListener(UnityAction<Beat> call)
        {
            _listenerCount--;
            base.RemoveListener(call);
        }
    }

    [System.Serializable]
    public class OnSubBeat : UnityEvent<Beat, int>
    {
        private int _listenerCount = 0;
        public int listenerCount { get { return _listenerCount + GetPersistentEventCount(); } }

        /// <summary>
        /// beat, count
        /// </summary>
        /// <param name="call"></param>
        new public void AddListener(UnityAction<Beat, int> call)
        {
            _listenerCount++;
            base.AddListener(call);
        }

        new public void RemoveListener(UnityAction<Beat, int> call)
        {
            _listenerCount--;
            base.RemoveListener(call);
        }
    }

    [System.Serializable]
    public class TimingUpdate : UnityEvent<int, float, float, float>
    {
        private int _listenerCount = 0;
        public int listenerCount { get { return _listenerCount + GetPersistentEventCount(); } }

        /// <summary>
        /// index, interpolation, beatLength, beatTime
        /// </summary>
        /// <param name="call"></param>
        new public void AddListener(UnityAction<int, float, float, float> call)
        {
            _listenerCount++;
            base.AddListener(call);
        }

        new public void RemoveListener(UnityAction<int, float, float, float> call)
        {
            _listenerCount--;
            base.RemoveListener(call);
        }
    }

    [System.Serializable]
    public class OnFrameChanged : UnityEvent<int, int>
    {
        private int _listenerCount = 0;
        public int listenerCount { get { return _listenerCount + GetPersistentEventCount(); } }

        /// <summary>
        /// index, lastFrame
        /// </summary>
        new public void AddListener(UnityAction<int, int> call)
        {
            _listenerCount++;
            base.AddListener(call);
        }

        new public void RemoveListener(UnityAction<int, int> call)
        {
            _listenerCount--;
            base.RemoveListener(call);
        }
    }


    [System.Serializable]
    public class OnOnset : UnityEvent<OnsetType, Onset>
    {
        private int _listenerCount = 0;
        public int listenerCount { get { return _listenerCount + GetPersistentEventCount(); } }

        /// <summary>
        /// OnsetType, onset
        /// </summary>
        new public void AddListener(UnityAction<OnsetType, Onset> call)
        {
            _listenerCount++;
            base.AddListener(call);
        }

        new public void RemoveListener(UnityAction<OnsetType, Onset> call)
        {
            _listenerCount--;
            base.RemoveListener(call);
        }
    }

    [System.Serializable]
    public class OnChange : UnityEvent<int, float>
    {

        private int _listenerCount = 0;
        public int listenerCount { get { return _listenerCount + GetPersistentEventCount(); } }

        /// <summary>
        /// index, change
        /// </summary>
        new public void AddListener(UnityAction<int, float> call)
        {
            _listenerCount++;
            base.AddListener(call);
        }

        new public void RemoveListener(UnityAction<int, float> call)
        {
            _listenerCount--;
            base.RemoveListener(call);
        }
    }

    [System.Serializable]
    public class OnNewSong : UnityEvent<string, int>
    {

        private int _listenerCount = 0;
        public int listenerCount { get { return _listenerCount + GetPersistentEventCount(); } }

        /// <summary>
        /// name, totalFrames
        /// </summary>
        new public void AddListener(UnityAction<string, int> call)
        {
            _listenerCount++;
            base.AddListener(call);
        }

        new public void RemoveListener(UnityAction<string, int> call)
        {
            _listenerCount--;
            base.RemoveListener(call);
        }
    }

    /// <summary>
    /// beat
    /// </summary>
    public OnBeat onBeat = new OnBeat();

    /// <summary>
    /// beat, count
    /// </summary>
    public OnSubBeat onSubBeat = new OnSubBeat();

    /// <summary>
    /// OnsetType, onset
    /// </summary>
    public OnOnset onOnset = new OnOnset();

    /// <summary>
    /// index, change
    /// </summary>
    public OnChange onChange = new OnChange();

    /// <summary>
    /// index, interpolation, beatLength, beatTime
    /// </summary>
    public TimingUpdate timingUpdate = new TimingUpdate();

    /// <summary>
    /// index, lastFrame
    /// </summary>
    public OnFrameChanged onFrameChanged = new OnFrameChanged();

    /// <summary>
    /// name, totalFrames
    /// </summary>
    public OnNewSong onNewSong = new OnNewSong();

    private static List<RhythmEventProvider> eventProviderList = new List<RhythmEventProvider>();
    public static event Action<RhythmEventProvider> OnEventProviderEnabled;

    void OnEnable()
    {
        if (!eventProviderList.Contains(this))
        {
            eventProviderList.Add(this);

            if (OnEventProviderEnabled != null)
                OnEventProviderEnabled(this);
        }
    }

    void OnDisable()
    {
        if (eventProviderList.Contains(this))
            eventProviderList.Remove(this);
    }
}

