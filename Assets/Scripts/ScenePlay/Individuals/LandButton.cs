using System.Text;
using UnityEngine;
using TMPro;
using static Constants;

public class LandButton : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private TextMeshProUGUI _title = null;
    [SerializeField] private TextMeshProUGUI _resources = null;
    private Land _land = null;
    private byte _index = 0;
    private bool _isLand = true;



    /* ==================== Public Methods ==================== */

    public void ButtonLand()
    {
        Exploration.Instance.SetActive(false);
        LandScreen.Instance.ShowLandScreen(_land, (byte)(_index + 1), this);
        PlayManager.Instance.SetGamePause(true);
        AudioManager.Instance.Play(AudioType.Touch);
    }


    /// <summary>
    /// Sets land button info text.
    /// </summary>
    public void SetTexts()
    {
        _index = (byte)(PlayManager.Instance.Lands.Count - 1);
        _land = PlayManager.Instance.Lands[_index];
        OnLanguageChange();
        LanguageManager.Instance.AddOnLanguageChange(OnLanguageChange);
        gameObject.SetActive(true);
    }


    /// <summary>
    /// Sets city name on land button.
    /// </summary>
    /// <param name="name">The name of city</param>
    public void SetCityName(string name)
    {
        _title.text = name;
        _isLand = true;
    }



    /* ==================== Private Methods ==================== */

    private void OnLanguageChange()
    {
        // Title
        if (_isLand)
        {
            _title.text = $"{LanguageManager.Instance[TEX_LAND]} {(_index + 1).ToString()}";
        }

        // Info
        StringBuilder builder = new StringBuilder();
        if (_land.Stone > 0)
        {
            builder.Append($"{LanguageManager.Instance[TEX_STONE]} {_land.Stone.ToString()}\n");
        }
        if (_land.Iron > 0)
        {
            builder.Append($"{LanguageManager.Instance[TEX_IRON]} {_land.Iron.ToString()}\n");
        }
        if (_land.HeavyMetal > 0)
        {
            builder.Append($"{LanguageManager.Instance[TEX_HEAVY]} {_land.HeavyMetal.ToString()}\n");
        }
        if (_land.PreciousMetal > 0)
        {
            builder.Append($"{LanguageManager.Instance[TEX_PRECIOUS]} {_land.PreciousMetal.ToString()}\n");
        }
        if (_land.Nuclear > 0)
        {
            builder.Append($"{LanguageManager.Instance[TEX_NUCLEAR]} {_land.Nuclear.ToString()}\n");
        }
        builder.Remove(builder.Length - 1, 1);
        _resources.text = builder.ToString();
    }
}
