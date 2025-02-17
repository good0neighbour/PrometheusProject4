using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LanguageTranslator : MonoBehaviour
{
    /* ==================== Fields ==================== */

    private ushort _wordIndex = 0;



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// Gets the original word on Unity editor.
    /// </summary>
    /// <returns>Original word</returns>
    public string GetOriginalWord()
    {
        return GetComponent<TextMeshProUGUI>().text;
    }


    /// <summary>
    /// Sets word index.
    /// </summary>
    /// <param name="index">Word index</param>
    public void SetWordIndex(ushort index)
    {
        _wordIndex = index;
    }


    /// <summary>
    /// Gets its word index.
    /// </summary>
    /// <returns>Word index</returns>
    public ushort GetWordIndex()
    {
        return _wordIndex;
    }


    /// <summary>
    /// Sets word on UI.
    /// </summary>
    /// <param name="word">Translated word</param>
    public void SetUIWord(string word, TMP_FontAsset font)
    {
        TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = word;
        tmp.font = font;
    }
}
