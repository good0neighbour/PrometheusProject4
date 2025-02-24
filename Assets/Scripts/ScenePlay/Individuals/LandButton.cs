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
        _title.text = $"{LanguageManager.Instance["����"]} {_index.ToString()}";

        // Info
        StringBuilder builder = new StringBuilder();
        Land land = PlayManager.Instance.Lands[_index];
        if (land.Stone > 0)
        {
            builder.Append($"{LanguageManager.Instance["����"]} {land.Stone.ToString()}\n");
        }
        if (land.Iron > 0)
        {
            builder.Append($"{LanguageManager.Instance["ö"]} {land.Iron.ToString()}\n");
        }
        if (land.HeavyMetal > 0)
        {
            builder.Append($"{LanguageManager.Instance["�߱ݼ�"]} {land.HeavyMetal.ToString()}\n");
        }
        if (land.PreciousMetal > 0)
        {
            builder.Append($"{LanguageManager.Instance["�ͱݼ�"]} {land.PreciousMetal.ToString()}\n");
        }
        if (land.Nuclear > 0)
        {
            builder.Append($"{LanguageManager.Instance["�ٹ���"]} {land.Nuclear.ToString()}\n");
        }
        builder.Remove(builder.Length - 1, 1);
        _resources.text = builder.ToString();
    }
}
