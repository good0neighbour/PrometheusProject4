using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageManager
{
    /* ==================== Fields ==================== */

    static private LanguageManager _instance = null;
    private GameDelegate _onLanguageChange = null;
    private List<LanguageTranslator> _translators = new List<LanguageTranslator>();
    private string[] words = null;


    static public LanguageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LanguageManager();
            }
            return _instance;
        }
    }



    /* ==================== Indexer ==================== */

    public string this[ushort index]
    {
        get
        {
            return words[index];
        }
    }



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// Applies language setting to word on current scene. It should be called only once at the begining of a scene.
    /// </summary>
    public void LanguageInitialize(LanguageType language)
    {
        // UI language setting
        string[] words = LoadUILanguage(LanguageType.Korean);
        _translators.Clear();
        foreach (LanguageTranslator laTr in GameObject.FindObjectsByType<LanguageTranslator>(FindObjectsSortMode.None))
        {
            _translators.Add(laTr);
            laTr.SetWordIndex((ushort)Array.IndexOf(words, laTr.GetOriginalWord()));
        }

        // Adopt language setting
        LanguageChange(language);
    }


    /// <summary>
    /// Changes game language.
    /// </summary>
    /// <param name="language">Selected language</param>
    public void LanguageChange(LanguageType language)
    {
        // UI language change
        string[] words = LoadUILanguage(language);
        foreach (LanguageTranslator laTr in _translators)
        {
            laTr.SetUIWord(words[laTr.GetWordIndex()]);
        }

        // On language change
        _onLanguageChange?.Invoke();
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

    private string[] LoadUILanguage(LanguageType language)
    {
        return JsonUtility.FromJson<LanguageJson>(Resources.Load($"{language.ToString()}UI").ToString()).Words;
    }



    /* ==================== Structure ==================== */

    [Serializable]
    public struct LanguageJson
    {
        public string[] Words;
    }
}
