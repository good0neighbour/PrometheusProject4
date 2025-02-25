using UnityEngine;
using TMPro;
using System.Text;

public class LandButton : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private TextMeshProUGUI _title = null;
    [SerializeField] private TextMeshProUGUI _resources = null;
    private Land _land = null;



    /* ==================== Public Methods ==================== */

    public void SetTexts()
    {
        _land = PlayManager.Instance.Lands[PlayManager.Instance.Lands.Count - 1];
        OnLanguageChange();
        LanguageManager.Instance.AddOnLanguageChange(OnLanguageChange);
        gameObject.SetActive(true);
    }



    /* ==================== Private Methods ==================== */

    private void OnLanguageChange()
    {
        // Title
        _title.text = $"{LanguageManager.Instance["ÅäÁö"]} {PlayManager.Instance.Lands.Count.ToString()}";

        // Info
        StringBuilder builder = new StringBuilder();
        if (_land.Stone > 0)
        {
            builder.Append($"{LanguageManager.Instance["¼®Á¦"]} {_land.Stone.ToString()}\n");
        }
        if (_land.Iron > 0)
        {
            builder.Append($"{LanguageManager.Instance["Ã¶"]} {_land.Iron.ToString()}\n");
        }
        if (_land.HeavyMetal > 0)
        {
            builder.Append($"{LanguageManager.Instance["Áß±Ý¼Ó"]} {_land.HeavyMetal.ToString()}\n");
        }
        if (_land.PreciousMetal > 0)
        {
            builder.Append($"{LanguageManager.Instance["±Í±Ý¼Ó"]} {_land.PreciousMetal.ToString()}\n");
        }
        if (_land.Nuclear > 0)
        {
            builder.Append($"{LanguageManager.Instance["ÇÙ¹°Áú"]} {_land.Nuclear.ToString()}\n");
        }
        builder.Remove(builder.Length - 1, 1);
        _resources.text = builder.ToString();
    }
}
