using UnityEngine;

public class GameMenu : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private GameObject _languageScreen = null;



    /* ==================== Public Methods ==================== */

    public void ButtonGameMenuActive(bool active)
    {
        gameObject.SetActive(active);
    }


    public void ButtonLanguage()
    {
        _languageScreen.SetActive(true);
    }


    public void ButtonMainMenu()
    {

    }


    public void ButtonQuit()
    {
        Application.Quit();
    }
}
