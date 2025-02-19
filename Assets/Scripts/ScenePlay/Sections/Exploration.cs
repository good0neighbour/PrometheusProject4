using UnityEngine;
using UnityEngine.UI;

public class Exploration : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private Image _explorartionProgressBar = null;



    /* ==================== Private Methods ==================== */

    private void Update()
    {
        _explorartionProgressBar.fillAmount = PlayManager.Instance[FloatValue.Exploration];
    }
}
