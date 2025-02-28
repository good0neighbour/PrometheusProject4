using UnityEngine;
using TMPro;

public class CitiesInfo : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _title = null;
    [SerializeField] private TextMeshProUGUI _population = null;
    [SerializeField] private TextMeshProUGUI _popMov = null;
    [SerializeField] private TextMeshProUGUI _crime = null;
    [SerializeField] private TextMeshProUGUI _death = null;
    [SerializeField] private TextMeshProUGUI _fund = null;
    [SerializeField] private TextMeshProUGUI _research = null;
    [SerializeField] private TextMeshProUGUI _culture = null;
    [SerializeField] private TextMeshProUGUI _stability = null;
    [Header("City List")]
    [SerializeField] private GameObject _cityList = null;
    private City _curCity = null;

    static public CitiesInfo Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonFacilities()
    {
        if (_curCity == null)
        {
            return;
        }
        TechTreeScreen.Instance.ShowScreen(TechTreeType.Facilities);
        AudioManager.Instance.Play(AudioType.Touch);
    }


    /// <summary>
    /// Adds a city list.
    /// </summary>
    public void AddCity()
    {
        // Adds city list.
        Instantiate(_cityList, _cityList.transform.parent).GetComponent<CityButton>().SetTexts();

        // City screen button enable
        Territory.Instance.ShowScreenButtons();
    }


    /// <summary>
    /// Switches city info screen.
    /// </summary>
    /// <param name="city">City to show</param>
    /// <returns>Newly selected or already selected</returns>
    public bool SwitchCityInfo(City city)
    {
        // Already selected.
        if (_curCity == city)
        {
            return false;
        }

        // Switches city.
        _curCity = city;

        // City name
        _title.text = city.Name;

        // City info
        OnLateMonthChange();

#if UNITY_EDITOR
        Debug.Log($"Showing city: {_curCity.Name}");
#endif

        // Successfully switched.
        return true;
    }



    /* ==================== Private Methods ==================== */

    private void OnLateMonthChange()
    {
        if (gameObject.activeSelf)
        {
            _population.text = _curCity.Population.ToString("0");
            _popMov.text = _curCity.PopulationMovement.ToString("0");
            _crime.text = _curCity.Crime.ToString("0.00%");
            _death.text = _curCity.Death.ToString("0.00%");
            _fund.text = _curCity.AnnualFund.ToString("0");
            _research.text = _curCity.AnnualResearch.ToString("0");
            _culture.text = _curCity.AnnualCulture.ToString("0");
            _stability.text = _curCity.Stability.ToString("0");
        }
    }


    private void Awake()
    {
        // Unity singleton pattern
        Instance = this;
        gameObject.SetActive(false);
    }


    private void Start()
    {
        PlayManager.Instance.AddOnLateMonthChange(OnLateMonthChange);
    }


    private void OnEnable()
    {
        if (_curCity == null)
        {
            _curCity = PlayManager.Instance.Cities[0];
            _title.text = _curCity.Name;
        }
        OnLateMonthChange();
    }
}
