using System;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public partial class PlayManager : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [Header("Tech Tree Data")]
    [SerializeField] private TechTreeData _facilitiesData = null;
    [Header("Planet")]
    [SerializeField][Range(0.0f, -0.005f)] private float _surfaceRotation = -0.003125f;
    [SerializeField][Range(0.0f, -0.005f)] private float _cloudRotation = -0.0015625f;
    [SerializeField][Range(0.0f, 0.01f)] private float _lightRotation = 0.005f;
    private GameDelegate _onUpdate = null;
    private GameDelegate _onLateUpdate = null;
    private GameDelegate _onMonthChange = null;
    private GameDelegate _onLateMonthChange = null;
    private GameDelegate _onYearChange = null;
    private GameDelegate _onLateYearChange = null;
    private Material _planetMaterial = null;
    private PlayData _data = new PlayData();
    private float _planetSurfRot = 0.0f;
    private float _planetCloudRot = 0.0f;
    private float _planetlightRot = Mathf.PI;
    private float _time = 0.0f;
    private sbyte _gamePause = 0;
    private byte _gameSpeed = 1;

    static public PlayManager Instance { get; private set; }
    public List<Land> Lands { get; private set; }
    public List<City> Cities { get; private set; }
    public TechTreeData FacilitiesData { get { return _facilitiesData; } }
    public float DeltaTime { get; private set; }
    public bool GamePause { get; private set; }



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
    public void SetGamePause(bool pause)
    {
        // Error check
        if (pause)
        {
            // Adds the number of reason to pause game.
            ++_gamePause;
        }
        else
        {
            if (_gamePause <= 0)
            {
#if UNITY_EDITOR
                Debug.LogError("Too many resume calls.\nPlayManager - SetGamePause(GamePauseType pause)");
#endif
                return;
            }

            // Removes the number of reason to pause game.
            --_gamePause;
        }

        // Game pause or not
        if (_gamePause > 0)
        {
            GamePause = true;
        }
        else
        {
            GamePause = false;
        }

#if UNITY_EDITOR
        Debug.Log($"_gamePause: {_gamePause}");
#endif
    }


    /// <summary>
    /// Adds function to onUpdate delegate.
    /// </summary>
    /// <param name="onUpdate">Function to add</param>
    public void AddOnUpdate(GameDelegate onUpdate)
    {
        _onUpdate += onUpdate;
    }


    /// <summary>
    /// Adds function to onLateUpdate delegate.
    /// </summary>
    /// <param name="onLateUpdate">Function to add</param>
    public void AddOnLateUpdate(GameDelegate onLateUpdate)
    {
        _onLateUpdate += onLateUpdate;
    }


    /// <summary>
    /// Adds function to onMonthChange delegate.
    /// </summary>
    /// <param name="onMonthChange">Function to add</param>
    public void AddOnMonthChange(GameDelegate onMonthChange)
    {
        _onMonthChange += onMonthChange;
    }


    /// <summary>
    /// Adds function to onLateMonthChange delegate.
    /// </summary>
    /// <param name="onLateMonthChange">Function to add</param>
    public void AddOnLateMonthChange(GameDelegate onLateMonthChange)
    {
        _onLateMonthChange += onLateMonthChange;
    }


    /// <summary>
    /// Adds function to onYearChange delegate.
    /// </summary>
    /// <param name="onYearChange">Function to add</param>
    public void AddOnYearChange(GameDelegate onYearChange)
    {
        _onYearChange += onYearChange;
    }


    /// <summary>
    /// Adds function to onLateYearChange delegate.
    /// </summary>
    /// <param name="onLateYearChange">Function to add</param>
    public void AddOnLateYearChange(GameDelegate onLateYearChange)
    {
        _onLateYearChange += onLateYearChange;
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


    private void LoadPlayData()
    {
        SaveData data = Resources.Load<SaveData>("PlayData/InitialData");

        // Data
        _data = data.Variables;

        // Lands
        Lands = new List<Land>();
        if (data.Lands != null)
        {
            for (ushort i = 0; i < data.Lands.Count; ++i)
            {
                // List add
                Lands.Add(data.Lands[i]);

                // Land list UI
                Exploration.Instance.AddLand();
            }
        }

        // Cities
        Cities = new List<City>();
        if (data.Cities != null)
        {
            for (ushort i = 0; i < data.Cities.Count; ++i)
            {
                // City add
                Cities.Add(data.Cities[i]);

                // City list UI
                CitiesInfo.Instance.AddCity();
            }
        }
    }


    private void SavePlayData()
    {
        SaveData data = Resources.Load<SaveData>("PlayData/SaveData");
        data.Variables = _data;
        data.Lands = Lands;
    }


    private void Awake()
    {
        // Unity singleton pattern
        Instance = this;

        // Loads play data.
        LoadPlayData();

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
        // Game Pause
        if (GamePause)
        {
            return;
        }

        // Delta time
        DeltaTime = Time.deltaTime * _gameSpeed;

        // Update pass
        _onUpdate?.Invoke();

        #region Time Pass
        if (_time >= MONTH_PER_SEC)
        {
            _time -= MONTH_PER_SEC;

            // Monthly move
            _onMonthChange?.Invoke();

            if (_data.Month >= 12)
            {
                // Year pass
                ++_data.Year;
                _data.Month = 1;

                // Annual move
                _onYearChange?.Invoke();
                _onLateYearChange?.Invoke();
            }
            else
            {
                // Month pass
                ++_data.Month;
            }

            // Late monthly move
            _onLateMonthChange?.Invoke();
        }
        _time += DeltaTime;
        #endregion

        #region Exploration
        if (Lands.Count < MAX_LAND_NUM)
        {
            if (_data.Explore >= _data.ExploreAmn)
            {
                // Exploration update
                _data.Explore -= _data.ExploreAmn;
                _data.ExploreAmn *= _data.ExploreIncMlt;

                // New land
                Lands.Add(new Land().RandomResources());

                // Land list UI
                Exploration.Instance.AddLand();

                // Show Message
                MessageManager.Instance.EnqueueMessage(TEX_MES_NEWLAND);
            }
            _data.Explore += DeltaTime * _data.ExploreSpdMlt * _data.ExploreNum;
        }
        #endregion

        // Update pass
        _onLateUpdate?.Invoke();

        // Planet rotation
        PlanetRotation();
    }
}
