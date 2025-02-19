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



    /* ==================== Public Methods ==================== */

#if UNITY_EDITOR
    public void LoadAudioChannel()
	{
        if (AudioManager.Instance == null)
        {
            GameObject audio = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AudioChannel.prefab");
            Object.Instantiate(audio);
        }
    }
#endif
}
