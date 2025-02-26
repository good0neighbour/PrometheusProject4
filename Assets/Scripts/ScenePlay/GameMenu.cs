using UnityEngine;

public class GameMenu : MonoBehaviour
{
    /* ==================== Fields ==================== */

    static public GameMenu Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonGameMenuActive(bool active)
    {
        PlayManager.Instance.SetGamePause(active);
        gameObject.SetActive(active);
        AudioManager.Instance.Play(AudioType.Touch);
    }


    public void ButtonMainMenu()
    {

        AudioManager.Instance.Play(AudioType.Touch);
    }


    public void ButtonQuit()
    {
        Application.Quit();
    }



    /* ==================== Private Methods ==================== */

    private void Awake()
    {
        // Unity singleton pattern
        Instance = this;
    }


    private void Start()
    {
        gameObject.SetActive(false);
    }
}
