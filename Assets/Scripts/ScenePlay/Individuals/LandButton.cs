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
        _title.text = $"{LanguageManager.Instance["����"]} {PlayManager.Instance.Lands.Count.ToString()}";

        // Info
        StringBuilder builder = new StringBuilder();
        if (_land.Stone > 0)
        {
            builder.Append($"{LanguageManager.Instance["����"]} {_land.Stone.ToString()}\n");
        }
        if (_land.Iron > 0)
        {
            builder.Append($"{LanguageManager.Instance["ö"]} {_land.Iron.ToString()}\n");
        }
        if (_land.HeavyMetal > 0)
        {
            builder.Append($"{LanguageManager.Instance["�߱ݼ�"]} {_land.HeavyMetal.ToString()}\n");
        }
        if (_land.PreciousMetal > 0)
        {
            builder.Append($"{LanguageManager.Instance["�ͱݼ�"]} {_land.PreciousMetal.ToString()}\n");
        }
        if (_land.Nuclear > 0)
        {
            builder.Append($"{LanguageManager.Instance["�ٹ���"]} {_land.Nuclear.ToString()}\n");
        }
        builder.Remove(builder.Length - 1, 1);
        _resources.text = builder.ToString();
    }
}
