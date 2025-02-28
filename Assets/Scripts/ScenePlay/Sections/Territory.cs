using UnityEngine;

public class Territory : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private GameObject _exploration = null;
    [SerializeField] private GameObject _citiesInfo = null;
    [SerializeField] private GameObject[] _screenButtons = null;
    private bool _isExploration = true;

    static public Territory Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonScreen(bool isExploration)
    {
        // Already selected.
        if (_isExploration == isExploration)
        {
            return;
        }

        // Screen switch
        _isExploration = isExploration;
        _exploration.SetActive(isExploration);
        _citiesInfo.SetActive(!isExploration);

        // Audio play
        AudioManager.Instance.Play(AudioType.Touch);
    }


    /// <summary>
    /// Enables screen buttons.
    /// </summary>
    public void ShowScreenButtons()
    {
        _screenButtons[0].SetActive(true);
        _screenButtons[1].SetActive(true);
    }



    /* ==================== Private Methods ==================== */

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        gameObject.SetActive(false);
    }
}
