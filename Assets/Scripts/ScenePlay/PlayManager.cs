using UnityEngine;

public class PlayManager : MonoBehaviour
{
    /* ==================== Fields ==================== */



    /* ==================== Public Methods ==================== */



    /* ==================== Private Methods ==================== */

    private void Start()
    {
        LanguageManager.Instance.LanguageInitialize("Korean");
    }
}
