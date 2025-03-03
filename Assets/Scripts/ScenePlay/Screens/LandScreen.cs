using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;

public class LandScreen : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private List<ResourceBlock> _blocks = null;
    [SerializeField] private GameObject _landScreen = null;
    [SerializeField] private GameObject _cityBuildScreen = null;
    [SerializeField] private GameObject _cityBuildButton = null;
    [SerializeField] private TextMeshProUGUI _titleText = null;
    [SerializeField] private TextMeshProUGUI _cityBuildText = null;
    [SerializeField] private TMP_InputField _inputField = null;
    [SerializeField] private TextMeshProUGUI[] _cityTitles = null;
    [SerializeField] private TextMeshProUGUI[] _capTexts = null;
    [SerializeField] private TextMeshProUGUI[] _cosTexts = null;
    [SerializeField] private Image[] _cityTypeBackgrounds = null;
    private LandButton _landButton = null;
    private Land _land = null;
    private ushort _cost = 0;
    private ushort _capacity = 0;
    private byte _index = 0;
    private byte _landIndex = 0;
    private byte _selectedCity = byte.MaxValue;
    private bool _isLandScreen = true;
    private bool _cityAvailable = false;

    static public LandScreen Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonCityBuild()
    {
        string cityName = _inputField.text;

        if (_isLandScreen)
        {
            _landScreen.SetActive(false);
            _cityBuildScreen.SetActive(true);
            _isLandScreen = false;

            // Audio play
            AudioManager.Instance.Play(AudioType.Touch);
        }
        else if (_cityAvailable && !string.IsNullOrEmpty(cityName))
        {
            // Title Change
            _titleText.text = cityName;

            // Add city
            _land.City = (byte)PlayManager.Instance.Cities.Count;
            PlayManager.Instance.Cities.Add(new City(cityName, _landIndex, _capacity));
            CitiesInfo.Instance.AddCity();
            _landButton.SetCityName(cityName);

            // Cost
            PlayManager.Instance.Fund -= _cost;
            PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Fund);

            // Resources gain
            ResourcesGain();

            // Button disable
            _cityBuildButton.SetActive(false);

            // Back to land screen
            _cityBuildScreen.SetActive(false);
            _landScreen.SetActive(true);
            _isLandScreen = true;

            // Message
            MessageManager.Instance.EnqueueMessage(LanguageManager.Instance[TEX_MES_NEWCITY], cityName);

            // Audio play
            AudioManager.Instance.Play(AudioType.Select);
#if UNITY_EDITOR
            Debug.Log($"Current cities: {PlayManager.Instance.Cities.Count.ToString()}");
#endif
        }
        else
        {
            // Audio play
            AudioManager.Instance.Play(AudioType.Unable);
            return;
        }
    }


    public void ButtonBack()
    {
        if (_isLandScreen)
        {
            // Switch screen
            gameObject.SetActive(false);
            Exploration.Instance.SetActive(true);

            // Disable all blocks
            for (byte i = 0; i < _index; ++i)
            {
                _blocks[i].Block.SetActive(false);
            }
            _index = 0;

            // Initialize
            _inputField.text = null;
            _selectedCity = byte.MaxValue;
            _cityAvailable = false;
            _cityTypeBackgrounds[0].color = UNSELECTED;
            _cityTypeBackgrounds[1].color = UNSELECTED;
            _cityTypeBackgrounds[2].color = UNSELECTED;

            // Game resume
            PlayManager.Instance.SetGamePause(false);
        }
        else
        {
            _cityBuildScreen.SetActive(false);
            _landScreen.SetActive(true);
            _isLandScreen = true;
        }

        // Audio play
        AudioManager.Instance.Play(AudioType.Touch);
    }


    public void ButtonCityType(int index)
    {
        // Selected the same one.
        if (_selectedCity == index)
        {
            return;
        }
        else if (_selectedCity < byte.MaxValue)
        {
            // Unselects the previous one.
            _cityTypeBackgrounds[_selectedCity].color = UNSELECTED;
        }

        // Select the new one.
        _cityTypeBackgrounds[index].color = SELECTED;
        _selectedCity = (byte)index;

        // Cost check
        _cityAvailable = true;
        switch (index)
        {
            case 0:
                _cost = PlayManager.Instance.CityCostSmall;
                _capacity = PlayManager.Instance.CityCapaSmall;
                if (PlayManager.Instance.Fund < _cost)
                {
                    _cityAvailable = false;
                }
                break;
                
            case 1:
                _cost = PlayManager.Instance.CityCostMiddle;
                _capacity = PlayManager.Instance.CityCapaMiddle;
                if (PlayManager.Instance.Fund < _cost)
                {
                    _cityAvailable = false;
                }
                break;
                
            case 2:
                _cost = PlayManager.Instance.CityCostLarge;
                _capacity = PlayManager.Instance.CityCapaLarge;
                if (PlayManager.Instance.Fund < _cost)
                {
                    _cityAvailable = false;
                }
                break;

            default:
#if UNITY_EDITOR
                Debug.LogError($"Wrong city type number.\nLandScreen, ButtonCityType(int index), index: {index.ToString()}");
#endif
                break;
        }

        // Audio play
        AudioManager.Instance.Play(AudioType.Touch);
    }


    /// <summary>
    /// Shows land screen.
    /// </summary>
    /// <param name="land">Land to show.</param>
    public void ShowLandScreen(Land land, byte landIndex, LandButton button)
    {
        _index = 0;
        _land = land;
        _landIndex = landIndex;
        _landButton = button;

        // Info
        if (land.Stone > 0)
        {
            SetResourceBlock(
                LanguageManager.Instance[TEX_STONE],
                LanguageManager.Instance[TEX_DES_STONE],
                land.Stone.ToString()
            );
        }
        if (land.Iron > 0)
        {
            SetResourceBlock(
                LanguageManager.Instance[TEX_IRON],
                LanguageManager.Instance[TEX_DES_IRON],
                land.Iron.ToString()
            );
        }
        if (land.HeavyMetal > 0)
        {
            SetResourceBlock(
                LanguageManager.Instance[TEX_HEAVY],
                LanguageManager.Instance[TEX_DES_HEAVY],
                land.HeavyMetal.ToString()
            );
        }
        if (land.PreciousMetal > 0)
        {
            SetResourceBlock(
                LanguageManager.Instance[TEX_PRECIOUS],
                LanguageManager.Instance[TEX_DES_PRECIOUS],
                land.PreciousMetal.ToString()
            );
        }
        if (land.Nuclear > 0)
        {
            SetResourceBlock(
                LanguageManager.Instance[TEX_NUCLEAR],
                LanguageManager.Instance[TEX_DES_NUCLEAR],
                land.Nuclear.ToString()
            );
        }

        // Check city built
        if (land.City == byte.MaxValue)
        {
            // Title show
            _titleText.text = $"{LanguageManager.Instance[TEX_LAND]} {landIndex.ToString()}";

            // Button active
            _cityBuildButton.SetActive(true);

            // Cost check
            if (PlayManager.Instance.Fund < PlayManager.Instance.CityCostSmall)
            {
                _cityTitles[0].color = DISABLED;
            }
            else
            {
                _cityTitles[0].color = Color.white;
            }
            if (PlayManager.Instance.Fund < PlayManager.Instance.CityCostMiddle)
            {
                _cityTitles[1].color = DISABLED;
            }
            else
            {
                _cityTitles[1].color = Color.white;
            }
            if (PlayManager.Instance.Fund < PlayManager.Instance.CityCostLarge)
            {
                _cityTitles[2].color = DISABLED;
            }
            else
            {
                _cityTitles[2].color = Color.white;
            }

            // Info text
            _capTexts[0].text = $"{LanguageManager.Instance[TEX_CAPACITY]}\n{PlayManager.Instance.CityCapaSmall.ToString()}";
            _capTexts[1].text = $"{LanguageManager.Instance[TEX_CAPACITY]}\n{PlayManager.Instance.CityCapaMiddle.ToString()}";
            _capTexts[2].text = $"{LanguageManager.Instance[TEX_CAPACITY]}\n{PlayManager.Instance.CityCapaLarge.ToString()}";
            _cosTexts[0].text = $"{LanguageManager.Instance[TEX_COST]}\n{PlayManager.Instance.CityCostSmall.ToString()}";
            _cosTexts[1].text = $"{LanguageManager.Instance[TEX_COST]}\n{PlayManager.Instance.CityCostMiddle.ToString()}";
            _cosTexts[2].text = $"{LanguageManager.Instance[TEX_COST]}\n{PlayManager.Instance.CityCostLarge.ToString()}";
        }
        else
        {
            // Title show
            _titleText.text = $"{PlayManager.Instance.Cities[land.City].Name}";

            // Button disable
            _cityBuildButton.SetActive(false);
        }

        // Show
        gameObject.SetActive(true);
    }



    /* ==================== Private Methods ==================== */

    private void SetResourceBlock(string title, string des, string amount)
    {
        // Check block count
        if (_index >= _blocks.Count)
        {
            GameObject block = Instantiate(_blocks[0].Block, _blocks[0].Block.transform.parent);
            _blocks.Add(new ResourceBlock(
                block,
                block.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),
                block.transform.GetChild(1).GetComponent<TextMeshProUGUI>(),
                block.transform.GetChild(2).GetComponent<TextMeshProUGUI>()
            ));
#if UNITY_EDITOR
            Debug.Log("New resource block has been created.");
#endif
        }

        // Print info
        _blocks[_index].Title.text = title;
        _blocks[_index].Description.text = des;
        _blocks[_index].Amount.text = amount;
        _blocks[_index].Block.SetActive(true);

        // Next index
        ++_index;
    }


    private void ResourcesGain()
    {
        if (_land.Stone > 0)
        {
            PlayManager.Instance.StoneTotal += _land.Stone;
            PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Stone);
        }
        if (_land.Iron > 0)
        {
            PlayManager.Instance.IronTotal += _land.Iron;
            PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Iron);
        }
        if (_land.HeavyMetal > 0)
        {
            PlayManager.Instance.HeavyMetalTotal += _land.HeavyMetal;
            PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Heavy);
        }
        if (_land.PreciousMetal > 0)
        {
            PlayManager.Instance.PreciousMetalTotal += _land.PreciousMetal;
            PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Precious);
        }
        if (_land.Nuclear > 0)
        {
            PlayManager.Instance.NuclearTotal += _land.Nuclear;
            PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Nuclear);
        }
    }


    private void Awake()
    {
        // Unity singleton pattern
        Instance = this;
    }


    private void Start()
    {
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (_isLandScreen || (_cityAvailable && !string.IsNullOrEmpty(_inputField.text)))
        {
            _cityBuildText.color = Color.white;
        }
        else
        {
            _cityBuildText.color = DISABLED;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ButtonBack();
        }
    }



    /* ==================== Structure ==================== */

    [Serializable]
    private struct ResourceBlock
    {
        public GameObject Block;
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Amount;

        public ResourceBlock(GameObject block, TextMeshProUGUI title, TextMeshProUGUI des, TextMeshProUGUI amount)
        {
            Block = block;
            Title = title;
            Description = des;
            Amount = amount;
        }
    }
}
