using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;

public class Exploration : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [Header("UI")]
    [SerializeField] private Image _explorartionProgressBar = null;
    [SerializeField] private TextMeshProUGUI _explorationNumText = null;
    [SerializeField] private TextMeshProUGUI _explorerAddText = null;
    [SerializeField] private TextMeshProUGUI _explorationCostText = null;
    [Header("Land List")]
    [SerializeField] private GameObject _ButtonLand = null;

    static public Exploration Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonExplorationNum()
    {
        if (PlayManager.Instance.Fund >= PlayManager.Instance.ExploreCost)
        {
            // Adds the number of explorer.
            ++PlayManager.Instance.ExploreNum;

            // Cost
            PlayManager.Instance.Fund -= PlayManager.Instance.ExploreCost;

            // Updates explorer text.
            _explorationNumText.text = $"{LanguageManager.Instance[TEX_EXPLORER]} {PlayManager.Instance.ExploreNum.ToString()}";

            // Plays audio.
            AudioManager.Instance.Play(AudioType.Select);
        }
        else
        {
            // Plays audio.
            AudioManager.Instance.Play(AudioType.Unable);
        }

        // Cost check
        CostCheck();
    }


    /// <summary>
    /// Adds land list UI based on the last land.
    /// </summary>
    public void AddLand()
    {
        Instantiate(_ButtonLand, _ButtonLand.transform.parent).GetComponent<LandButton>().SetTexts();
    }


    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }



    /* ==================== Private Methods ==================== */

    private void OnLanguageChange()
    {
        _explorationNumText.text = $"{LanguageManager.Instance[TEX_EXPLORER]} {PlayManager.Instance.ExploreNum}";
        _explorationCostText.text = $"{LanguageManager.Instance[TEX_FUND]} {PlayManager.Instance.ExploreNum.ToString()}";
    }


    private void CostCheck()
    {
        if (PlayManager.Instance.Fund < PlayManager.Instance.ExploreCost)
        {
            _explorerAddText.color = DISABLED;
            _explorationCostText.color = DISABLED;
        }
        else
        {
            _explorerAddText.color = Color.white;
            _explorationCostText.color = Color.white;
        }
    }


    private void Awake()
    {
        // Unity singleton pattern
        Instance = this;

        // Language set
        OnLanguageChange();
        LanguageManager.Instance.AddOnLanguageChange(OnLanguageChange);
    }


    private void Update()
    {
        // Exploration progress
        _explorartionProgressBar.fillAmount = PlayManager.Instance.Explore / PlayManager.Instance.ExploreAmn;

        // Cost check
        CostCheck();
    }
}
