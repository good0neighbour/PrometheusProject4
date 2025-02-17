using UnityEngine;

public class GameMenu : MonoBehaviour
{
    /* ==================== Fields ==================== */



    /* ==================== Public Methods ==================== */

    public void ButtonResume()
    {
        gameObject.SetActive(false);
    }


    public void ButtonSettings()
    {

    }


    public void ButtonMainMenu()
    {

    }


    public void ButtonQuit()
    {
        Application.Quit();
    }
}
