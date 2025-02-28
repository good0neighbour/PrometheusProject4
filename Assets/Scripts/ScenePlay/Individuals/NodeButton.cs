using UnityEngine;

public class NodeButton : MonoBehaviour
{
    /* ==================== Fields ==================== */

    public byte Index { get; set; }



    /* ==================== Public Methods ==================== */

    public void ButtonNode()
    {
        TechTreeScreen.Instance.ShowNodeInfo(Index);
        TechTreeScreen.Instance.SetCursorPosition(transform.localPosition);
    }
}
