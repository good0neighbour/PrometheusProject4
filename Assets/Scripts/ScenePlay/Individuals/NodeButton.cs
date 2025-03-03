using UnityEngine;

public class NodeButton : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private GameObject _unadopted = null;
    [SerializeField] private GameObject _adopted = null;

    public byte Index { get; set; }



    /* ==================== Public Methods ==================== */

    public void ButtonNode()
    {
        TechTreeScreen.Instance.ShowNodeInfo(Index);
        TechTreeScreen.Instance.SetCursorPosition(transform.localPosition);
    }


    /// <summary>
    /// Switches node image.
    /// </summary>
    /// <param name="status">Node status</param>
    public void SetNodeStatus(TechStatus status)
    {
        switch (status)
        {
            case TechStatus.Adopted:
                _unadopted.SetActive(false);
                _adopted.SetActive(true);
                gameObject.SetActive(true);
                break;

            case TechStatus.Available:
                _unadopted.SetActive(true);
                _adopted.SetActive(false);
                gameObject.SetActive(true);
                break;

            case TechStatus.Unavailable:
                _unadopted.SetActive(false);
                _adopted.SetActive(false);
                gameObject.SetActive(false);
                break;

            default:
#if UNITY_EDITOR
                Debug.LogError($"Wrong node status.\nNodeButton, NodeStatus(TechStatus status), status: {status.ToString()}");
#endif
                break;
        }
    }
}
