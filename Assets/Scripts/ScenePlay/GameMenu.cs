using UnityEngine;

public class GameMenu : MonoBehaviour
{
    /* ==================== Public Methods ==================== */

    public void ButtonGameMenuActive(bool active)
    {
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
}
