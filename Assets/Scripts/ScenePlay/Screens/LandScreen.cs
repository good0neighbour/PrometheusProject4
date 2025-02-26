using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Constants;

public class LandScreen : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private List<ResourceBlock> _blocks = null;
    [SerializeField] private GameObject _landScreen = null;
    [SerializeField] private GameObject _cityBuildScreen = null;
    private Land _land = null;
    private byte _index = 0;
    private bool _isLandScreen = true;
    private bool _cityAvailable = false;

    static public LandScreen Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonCityBuild()
    {
        if (_isLandScreen)
        {
            _landScreen.SetActive(false);
            _cityBuildScreen.SetActive(true);
            _isLandScreen = false;
        }
        else if (_cityAvailable)
        {
            
        }
        else
        {
            AudioManager.Instance.Play(AudioType.Unable);
            return;
        }

        // Audio play
        AudioManager.Instance.Play(AudioType.Touch);
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


    /// <summary>
    /// Shows land screen.
    /// </summary>
    /// <param name="land">Land to show.</param>
    public void ShowLandScreen(Land land)
    {
        _index = 0;
        _land = land;

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
