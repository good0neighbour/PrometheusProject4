using UnityEngine;
using TMPro;
using static Constants;

public class CityButton : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private TextMeshProUGUI _title = null;
    [SerializeField] private TextMeshProUGUI _info = null;
    private City _city = null;
    private byte _index = 0;



    /* ==================== Public Methods ==================== */

    public void ButtonCity()
    {
        // Switch city info
        if (CitiesInfo.Instance.SwitchCityInfo(_city))
        {
            // Audio play
            AudioManager.Instance.Play(AudioType.Touch);
        }
    }


    /// <summary>
    /// Sets city button info text.
    /// </summary>
    public void SetTexts()
    {
        _index = (byte)(PlayManager.Instance.Cities.Count - 1);
        _city = PlayManager.Instance.Cities[_index];
        _title.text = _city.Name;
        OnLanguageChange();
        LanguageManager.Instance.AddOnLanguageChange(OnLanguageChange);
        PlayManager.Instance.AddOnLateMonthChange(OnLanguageChange);
        gameObject.SetActive(true);
    }



    /* ==================== Private Methods ==================== */

    private void OnLanguageChange()
    {
        if (gameObject.activeSelf)
        {
            _info.text = $"{LanguageManager.Instance[TEX_POPULATION]} {_city.Population.ToString("0")}\n{LanguageManager.Instance[TEX_STABILITY]} {_city.Stability.ToString("0")}";
        }
    }


    private void OnEnable()
    {
        OnLanguageChange();
    }
}
