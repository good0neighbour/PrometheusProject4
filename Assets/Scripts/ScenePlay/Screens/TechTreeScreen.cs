using UnityEngine;

public class TechTreeScreen : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private TechContentBase[] _contents = null;

    static public TechTreeScreen Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonBack()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.Play(AudioType.Touch);
    }


    /// <summary>
    /// Shows techtree screen.
    /// </summary>
    /// <param name="type">Content type to show.</param>
    public void ShowScreen(TechTreeType type)
    {
        // Content show
        _contents[(int)type].ShowContent();

        // Screen active
        gameObject.SetActive(true);
    }



    /* ==================== Private Methods ==================== */

    private void Awake()
    {
        // Unity singleton pattern
        Instance = this;
    }
}
