using UnityEngine;
using TMPro;

public class ListFunction : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private Vector2 _expandedAnchors = Vector2.zero;
    [SerializeField] private TextMeshProUGUI _expandText = null;
    private RectTransform _transform = null;
    private Vector2 _initialAnchors = Vector2.zero;
    private bool _isExpanded = false;



    /* ==================== Public Methods ==================== */

    public void ButtonExpland()
    {
        if (_isExpanded)
        {
            _transform.anchorMax = _initialAnchors;
            _expandText.text = "<";
            _isExpanded = false;
        }
        else
        {
            _transform.anchorMax = _expandedAnchors;
            _expandText.text = ">";
            _isExpanded = true;
        }

        AudioManager.Instance.Play(AudioType.Touch);
    }



    /* ==================== Private Methods ==================== */

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _initialAnchors = _transform.anchorMax;
    }
}
