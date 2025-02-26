using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageManager
{
    /* ==================== Fields ==================== */

    static private LanguageManager _instance = null;
    private GameDelegate _onLanguageChange = null;
    private Dictionary<string, string> _words = new Dictionary<string, string>();

    static public LanguageManager Instance
    {
        get
        {
            switch (_instance)
            {
                case null:
                    _instance = new LanguageManager();
                    return _instance;
                default:
                    return _instance;
            }
        }
    }



    /* ==================== Indexer ==================== */

    /// <summary>
    /// Returns current runtime language.
    /// </summary>
    /// <param name="korean">Word as Korean.</param>
    /// <returns>Current runtime language.</returns>
    public string this[string korean]
    {
        get
        {
            return _words[korean];
        }
    }



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// Applies language setting to word on current scene. It should be called only once at the begining of a scene.
    /// </summary>
    public void LanguageInitialize(string language)
    {
        // UI language setting
        string[] words = LoadUILanguage("Korean");
        List<LanguageTranslator> translators = new List<LanguageTranslator>();
        foreach (LanguageTranslator laTr in UnityEngine.Object.FindObjectsByType<LanguageTranslator>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            translators.Add(laTr);
            laTr.SetWordIndex((ushort)Array.IndexOf(words, laTr.GetOriginalWord()));
        }

        // Adopt language setting
        LanguageChange(
            language,
            translators.ToArray(),
            UnityEngine.Object.FindObjectsByType<FontSwitcher>(FindObjectsInactive.Include, FindObjectsSortMode.None)
        );
    }


    /// <summary>
    /// Changes game language.
    /// </summary>
    /// <param name="language">Selected language</param>
    public void LanguageChange(string language)
    {
        LanguageChange(
            language,
            UnityEngine.Object.FindObjectsByType<LanguageTranslator>(FindObjectsInactive.Include, FindObjectsSortMode.None),
            UnityEngine.Object.FindObjectsByType<FontSwitcher>(FindObjectsInactive.Include, FindObjectsSortMode.None)
        );
    }


    /// <summary>
    /// Adds function to onLanguageChange delegate.
    /// </summary>
    /// <param name="onLanguageChange">Function to add</param>
    public void AddOnLanguageChange(GameDelegate onLanguageChange)
    {
        _onLanguageChange += onLanguageChange;
    }


    /// <summary>
    /// Removes all registered functions in onLanguageChange.
    /// </summary>
    public void ClearOnLanguageChange()
    {
        _onLanguageChange = null;
    }



    /* ==================== Private Methods ==================== */

    private LanguageManager()
    {
        LanguageJson korean = JsonUtility.FromJson<LanguageJson>(Resources.Load("Languages/KoreanRuntime").ToString());
        LanguageJson cur = JsonUtility.FromJson<LanguageJson>(Resources.Load($"Languages/{GameManager.Instance.Language}Runtime").ToString());

        for (ushort i = 0; i < korean.Words.Length; ++i)
        {
#if UNITY_EDITOR
            if (_words.ContainsKey(korean.Words[i]))
            {
                Debug.LogError($"\"{korean.Words[i]}\" key already exists.\nLanguageManager - LanguageManager()");
                continue;
            }
#endif
            _words.Add(korean.Words[i], cur.Words[i]);
        }
    }


    private void LanguageChange(string language, LanguageTranslator[] translators, FontSwitcher[] switcher)
    {
        // UI language change
        string[] words = LoadUILanguage(language);
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>($"Fonts/Font{language}");
        foreach (LanguageTranslator laTr in translators)
        {
            laTr.SetUIWord(words[laTr.GetWordIndex()], font);
        }

        // Font change
        foreach (FontSwitcher fs in switcher)
        {
            fs.SwitchFont(font);
        }

        // Runtime language change
        LoadRuntimeLanguage(language);

        // On language change
        _onLanguageChange?.Invoke();
    }


    private string[] LoadUILanguage(string language)
    {
        return JsonUtility.FromJson<LanguageJson>(Resources.Load($"Languages/{language}UI").ToString()).Words;
    }


    private void LoadRuntimeLanguage(string language)
    {
        LanguageJson korean = JsonUtility.FromJson<LanguageJson>(Resources.Load("Languages/KoreanRuntime").ToString());
        LanguageJson cur = JsonUtility.FromJson<LanguageJson>(Resources.Load($"Languages/{language}Runtime").ToString());

        for (ushort i = 0; i < korean.Words.Length; ++i)
        {
            _words[korean.Words[i]] = cur.Words[i];
        }
    }



    /* ==================== Structure ==================== */

    [Serializable]
    public struct LanguageJson
    {
        public string[] Words;
    }
}
