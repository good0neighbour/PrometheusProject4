using System;
using UnityEngine;
using static Constants;

public class PlayManager : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [Header("Planet")]
    [SerializeField][Range(0.0f, -0.005f)] private float _surfaceRotation = -0.003125f;
    [SerializeField][Range(0.0f, -0.005f)] private float _cloudRotation = -0.0015625f;
    [SerializeField][Range(0.0f, 0.01f)] private float _lightRotation = 0.005f;
    private Material _planetMaterial = null;
    private PlayData _playData = new PlayData();
    private float _planetSurfRot = 0.0f;
    private float _planetCloudRot = 0.0f;
    private float _planetlightRot = Mathf.PI;
    private byte _gameSpeed = 1;
    private byte _gamePause = 0;

    static public PlayManager Instance { get; private set; }
    static public float DeltaTime { get; private set; }



    /* ==================== Indexer ==================== */

    public float this[FloatValue floatValue]
    {
        get
        {
            return _playData.FloatValues[(int)floatValue];
        }
        set
        {
            _playData.FloatValues[(int)floatValue] = value;
        }
    }

    public int this[IntValue intValue]
    {
        get
        {
            return _playData.IntValues[(int)intValue];
        }
        set
        {
            _playData.IntValues[(int)intValue] = value;
        }
    }

    public short this[ShortValue intValue]
    {
        get
        {
            return _playData.ShortValues[(int)intValue];
        }
        set
        {
            _playData.ShortValues[(int)intValue] = value;
        }
    }

    public byte this[ByteValue intValue]
    {
        get
        {
            return _playData.ByteValues[(int)intValue];
        }
        set
        {
            _playData.ByteValues[(int)intValue] = value;
        }
    }



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// Increases game speed.
    /// </summary>
    /// <returns>Returns game speed.</returns>
    public byte SetGameSpeed()
    {
        // Game speed change
        if (_gameSpeed == MAX_GAME_SPEED)
        {
            _gameSpeed = 1;
        }
        else
        {
            ++_gameSpeed;
        }

#if UNITY_EDITOR
        Debug.Log($"_gameSpeed: {_gameSpeed}");
#endif

        // Returns game speed.
        return _gameSpeed;
    }


    /// <summary>
    /// Adds the number of reason to pause game.
    /// </summary>
    /// <param name="pause">Pause or resume</param>
    public void SetGamePause(GamePause pause)
    {
        // Adds the number of reason to pause game.
        _gamePause += (byte)pause;

#if UNITY_EDITOR
        Debug.Log($"_gamePause: {_gamePause}");
#endif
    }



    /* ==================== Private Methods ==================== */

    private void PlanetRotation()
    {
        // Rotation amound
        _planetSurfRot += _surfaceRotation * DeltaTime;
        _planetCloudRot += _cloudRotation * DeltaTime;
        _planetlightRot += _lightRotation * DeltaTime;

        // Material set
        _planetMaterial.SetFloat("_SurfRotation", _planetSurfRot);
        _planetMaterial.SetFloat("_CloudRotation", _planetCloudRot);
        _planetMaterial.SetFloat("_LightDir", _planetlightRot);
    }


    private void Awake()
    {
        // Unity singleton pattern
        Instance = this;

        // Find planet renderer
        _planetMaterial = GetComponent<MeshRenderer>().material;
#if UNITY_EDITOR
        Debug.Log($"Planet material found: {_planetMaterial.name}");
#endif
#if UNITY_EDITOR
        // Loads AudioChannel on editor mode.
        GameManager.Instance.LoadAudioChannel();
#endif
    }


    private void Start()
    {
        LanguageManager.Instance.LanguageInitialize("Korean");
    }


    private void Update()
    {
        // Game pause
        switch (_gamePause)
        {
            case 0:
                break;

            default:
                DeltaTime = 0.0f;
                return;
        }

        // Delta time
        DeltaTime = Time.deltaTime * _gameSpeed;

        PlanetRotation();
    }



    /* ==================== Structure ==================== */

    [Serializable]
    private struct PlayData
    {
        public float[] FloatValues;
        public int[] IntValues;
        public short[] ShortValues;
        public byte[] ByteValues;
    }
}
