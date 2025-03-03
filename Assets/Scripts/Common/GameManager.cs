using System.Text;
using UnityEngine;
using UnityEditor;

public delegate void GameDelegate();

public class GameManager
{
    /* ==================== Fields ==================== */

    static private GameManager _instance = null;

    static public GameManager Instance
    {
        get
        {
            switch (_instance)
            {
                case null:
                    _instance = new GameManager();
                    return _instance;
                default:
                    return _instance;
            }
        }
    }

    public StringBuilder Builder { get; private set; }
    public string Language { get; set; }



    /* ==================== Public Methods ==================== */

#if UNITY_EDITOR
    public void LoadAudioChannel()
    {
        if (AudioManager.Instance == null)
        {
            Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AudioChannel.prefab"));
            Debug.Log("AudioChannel has been automatically created.");
        }
    }
#endif



    /* ==================== Private Methods ==================== */

    private GameManager()
    {
        Builder = new StringBuilder();
        Language = "Korean";
    }
}
