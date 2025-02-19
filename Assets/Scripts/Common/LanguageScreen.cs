using UnityEngine;

public class LanguageScreen : MonoBehaviour
{
    /* ==================== Public Methods ==================== */

    public void ButtonLanguageScreenActive(bool active)
    {
        gameObject.SetActive(active);
        AudioManager.Instance.Play(AudioType.Touch);
    }


    public void ButtonLanguage(string language)
    {
        LanguageManager.Instance.LanguageChange(language);
        gameObject.SetActive(false);
    }
}
