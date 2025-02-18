using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private byte _numOfChannel = 8;
    [SerializeField] private AudioData[] _audioDatas = null;
    private AudioSource[] _channels = null;
    private AudioSource _bgMusicChannel = null;
    private byte _curChnl = 0;


    static public AudioManager Instance { get; private set; }


#if UNITY_EDITOR
    public byte NumOfChannel
    {
        get
        {
            return _numOfChannel;
        }
        set
        {
            _numOfChannel = value;
        }
    }

    public AudioData[] AudioDatas {
        get
        {
            return _audioDatas;
        }
        set
        {
            _audioDatas = value;
        }
    }
#endif



    /* ==================== Public Methods ==================== */

    public void Play(AudioType type)
    {
        // Sets audio.
        _channels[_curChnl].clip = _audioDatas[(int)type].Clip;
        _channels[_curChnl].volume = _audioDatas[(int)type].Volume;

        // Play
        _channels[_curChnl].Play();

        // Next channel
        ++_curChnl;
        if (_curChnl >= _numOfChannel)
        {
            _curChnl = 0;
        }
    }



    /* ==================== Private Methods ==================== */

    private AudioSource CopyProperties(AudioSource target)
    {
        target.outputAudioMixerGroup = _bgMusicChannel.outputAudioMixerGroup;
        target.mute = _bgMusicChannel.mute;
        target.bypassEffects = _bgMusicChannel.bypassEffects;
        target.bypassListenerEffects = _bgMusicChannel.bypassListenerEffects;
        target.bypassReverbZones = _bgMusicChannel.bypassReverbZones;
        target.playOnAwake = _bgMusicChannel.playOnAwake;
        target.loop = _bgMusicChannel.loop;
        target.priority = _bgMusicChannel.priority;
        target.pitch = _bgMusicChannel.pitch;
        target.panStereo = _bgMusicChannel.panStereo;
        target.spatialBlend = _bgMusicChannel.spatialBlend;
        target.reverbZoneMix = _bgMusicChannel.reverbZoneMix;
        target.dopplerLevel = _bgMusicChannel.dopplerLevel;
        target.spread = _bgMusicChannel.spread;
        target.rolloffMode = _bgMusicChannel.rolloffMode;
        target.minDistance = _bgMusicChannel.minDistance;
        target.maxDistance = _bgMusicChannel.maxDistance;

        return target;
    }


    private void Awake()
    {
        // Unity singletop pattern
        Instance = this;

        // Original AudioSource
        _bgMusicChannel = GetComponent<AudioSource>();

        // Create channels
        _channels = new AudioSource[_numOfChannel];
        for (byte i = 0; i < _numOfChannel; ++i)
        {
            GameObject chnl = new GameObject($"SoundChannel_{i}");
            chnl.transform.SetParent(transform);
            _channels[i] = CopyProperties(chnl.AddComponent<AudioSource>());

            // Unity crashes
            //_channels[i] = Instantiate(_bgMusicChannel, transform);
        }
    }



    /* ==================== Structure ==================== */

    [Serializable]
#if UNITY_EDITOR
    public
#else
    private
#endif
    struct AudioData
    {
        public AudioClip Clip;
        public float Volume;
    }
}
