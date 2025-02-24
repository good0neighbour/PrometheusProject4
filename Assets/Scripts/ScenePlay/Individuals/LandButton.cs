using UnityEngine;
using TMPro;
using System.Text;

public class LandButton : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private TextMeshProUGUI _title = null;
    [SerializeField] private TextMeshProUGUI _resources = null;
    private ushort _index = 0;



    /* ==================== Public Methods ==================== */

    public void SetTexts()
    {
        _index = (ushort)(PlayManager.Instance.Lands.Count - 1);
        OnLanguageChange();
        LanguageManager.Instance.AddOnLanguageChange(OnLanguageChange);
        gameObject.SetActive(true);
    }



    /* ==================== Private Methods ==================== */

    private void OnLanguageChange()
    {
        // Title
        _title.text = $"{LanguageManager.Instance["ÅäÁö"]} {_index.ToString()}";

        // Info
        StringBuilder builder = new StringBuilder();
        Land land = PlayManager.Instance.Lands[_index];
        if (land.Stone > 0)
        {
            builder.Append($"{LanguageManager.Instance["¼®Á¦"]} {land.Stone.ToString()}\n");
        }
        if (land.Iron > 0)
        {
            builder.Append($"{LanguageManager.Instance["Ã¶"]} {land.Iron.ToString()}\n");
        }
        if (land.HeavyMetal > 0)
        {
            builder.Append($"{LanguageManager.Instance["Áß±Ý¼Ó"]} {land.HeavyMetal.ToString()}\n");
        }
        if (land.PreciousMetal > 0)
        {
            builder.Append($"{LanguageManager.Instance["±Í±Ý¼Ó"]} {land.PreciousMetal.ToString()}\n");
        }
        if (land.Nuclear > 0)
        {
            builder.Append($"{LanguageManager.Instance["ÇÙ¹°Áú"]} {land.Nuclear.ToString()}\n");
        }
        builder.Remove(builder.Length - 1, 1);
        _resources.text = builder.ToString();
    }
}
