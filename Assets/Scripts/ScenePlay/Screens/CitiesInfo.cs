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

    static public CitiesInfo Instance { get; private set; }
    public City CurrentCity { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonFacilities()
    {
        if (CurrentCity == null)
        {
            return;
        }
        TechTreeScreen.Instance.ShowScreen(
            TechTreeType.Facilities,
            PlayManager.Instance.FacilitySupportRate,
            CurrentCity.Name
        );
        PlayManager.Instance.SetGamePause(true);
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
        if (CurrentCity == city)
        {
            return false;
        }

        // Switches city.
        CurrentCity = city;

        // City name
        _title.text = city.Name;

        // City info
        OnLateMonthChange();

#if UNITY_EDITOR
        Debug.Log($"Showing city: {CurrentCity.Name}");
#endif

        // Successfully switched.
        return true;
    }



    /* ==================== Private Methods ==================== */

    private void OnLateMonthChange()
    {
        if (gameObject.activeSelf)
        {
            _population.text = CurrentCity.Population.ToString("0");
            _popMov.text = CurrentCity.PopulationMovement.ToString("0");
            _crime.text = CurrentCity.Crime.ToString("0.00%");
            _death.text = CurrentCity.Death.ToString("0.00%");
            _fund.text = CurrentCity.AnnualFund.ToString("0");
            _research.text = CurrentCity.AnnualResearch.ToString("0");
            _culture.text = CurrentCity.AnnualCulture.ToString("0");
            _stability.text = CurrentCity.Stability.ToString("0");
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
        if (CurrentCity == null)
        {
            CurrentCity = PlayManager.Instance.Cities[0];
            _title.text = CurrentCity.Name;
        }
        OnLateMonthChange();
    }
}
