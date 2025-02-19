using UnityEngine;

public class PlayMenuButton : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private bool _isLeftMenu = true;
    [SerializeField] private GameObject _targetMenu = null;



    /* ==================== Public Methods ==================== */

    public void ButtonMenu()
    {
#if UNITY_EDITOR
        if (_targetMenu == null)
        {
            Debug.LogError($"SelectedMenu is null.\n{name}, PlayManager, ButtonMenu(bool isLeft, GameObject selectedMenu), selectedMenu: {_targetMenu.ToString()}");
            return;
        }
#endif
        PlayMenuManager.Instance.MenuMove(_isLeftMenu, _targetMenu);
    }
}
