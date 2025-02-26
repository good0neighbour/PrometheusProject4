using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FontSwitcher : MonoBehaviour
{
    public void SwitchFont(TMP_FontAsset font)
    {
        GetComponent<TextMeshProUGUI>().font = font;
    }
}
