using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
using static UnityEditor.EditorGUILayout;

public class LanguageCreator : EditorWindow
{
    static private LanguageCreator _window = null;
    private FileInfo[] _languageData = null;
    private string[] _curTras = null;
    private string[] _curLans = null;
    private byte _status = 0;


    [MenuItem("PrometheusMission/Languages", priority = 0)]
    static void Open()
    {
        if (null == _window)
        {
            _window = CreateInstance<LanguageCreator>();
            _window.position = new Rect(700.0f, 300.0f, 300.0f, 500.0f);
        }

        // Searches language files.
        _window.SearchCurrentLanguageFiles();


        _window.ShowAuxWindow();
    }


    private void OnGUI()
    {
        // Title
        LabelField("Language Creator", EditorStyles.boldLabel);
        Space(10.0f);

        if (GUILayout.Button("Create UI Language files from Scenes"))
        {
            string[] words = null;

            // File create
            _status = UIJsonCreate(out words);
            if (_status == 1)
            {
                _status = ForGoogleTranslate(words);
            }

            // Searches language files again.
            SearchCurrentLanguageFiles();

            // Refresh
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Create Language Json based on translate files"))
        {
            // Creates json
            _status = TextToJson();

            // Searches language files again.
            SearchCurrentLanguageFiles();

            // Refresh
            AssetDatabase.Refresh();
        }

        // Print status
        switch (_status)
        {
            case 1:
                LabelField("Successfully created.");
                break;

            case 2:
                LabelField("Failed to create UI Language json.");
                break;

            case 3:
                LabelField("Failed to create translate file.");
                break;

            case 4:
                LabelField("Failed to create language files.");
                break;

            default:
                LabelField("");
                break;
        }

        Space(10.0f);

        // Language info
        PrintLanguageInfo(120.0f);
    }


    private byte UIJsonCreate(out string[] wordsData)
    {
        try
        {
            List<string> words = new List<string>();
            LanguageTranslator[] translators = null;
            string wordGot = null;

            // Memorizes current scene.
            Scene curScene = SceneManager.GetActiveScene();

            // Gets words.
            for (byte i = 0; i < SceneManager.sceneCount; ++i)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(i));
                translators = FindObjectsByType<LanguageTranslator>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (LanguageTranslator tra in translators)
                {
                    wordGot = tra.GetComponent<TextMeshProUGUI>().text;
                    if (!words.Contains(wordGot))
                    {
                        words.Add(wordGot);
                    }
                }
            }

            // Words data
            LanguageManager.LanguageJson jsonData;
            jsonData.Words = words.ToArray();
            wordsData = jsonData.Words;

            // Creates json file.
            File.WriteAllText(
                $"{Application.dataPath}/Resources/Languages/KoreanUI.json",
                JsonUtility.ToJson(jsonData, true)
            );

            // Backs to the original scene.
            SceneManager.SetActiveScene(curScene);

            // Successful
            return 1;
        }
        catch
        {
            // Failed
            wordsData = null;
            return 2;
        }
    }


    private byte ForGoogleTranslate(string[] words)
    {
        try
        {
            // Sets text content
            StringBuilder translateFile = new StringBuilder();
            foreach (string text in words)
            {
                translateFile.Append($"{text}\n");
            }

            // Creates translate file.
            File.WriteAllText($"{Application.dataPath}/LanguageData/TranslateUI.txt", translateFile.ToString());

            // Successful
            return 1;
        }
        catch
        {
            // Failed
            return 3;
        }
    }


    private byte TextToJson()
    {
        try
        {
            foreach (FileInfo file in _languageData)
            {
                try
                {
                    // Doesn't make translate file into json.
                    if (file.Name.Equals("TranslateUI.txt"))
                    {
                        break;
                    }

                    // Reads texts.
                    string text = File.ReadAllText(file.FullName);
                    if (!text.EndsWith('\n'))
                    {
                        text += '\n';
                    }

                    // Text save
                    List<string> words = new List<string>();
                    StringBuilder record = new StringBuilder();
                    foreach (char ch in text)
                    {
                        switch (ch)
                        {
                            case '\n':
                                words.Add(record.ToString());
                                record.Clear();
                                break;

                            case '\r':
                                break;

                            default:
                                record.Append(ch);
                                break;
                        }
                    }

                    // Creates json.
                    LanguageManager.LanguageJson jsonData;
                    jsonData.Words = words.ToArray();
                    File.WriteAllText(
                        $"{Application.dataPath}/Resources/Languages/{file.Name.Replace("txt", "json")}",
                        JsonUtility.ToJson(jsonData, true)
                    );
                }
                catch
                {
                    // Move on.
                    continue;
                }
            }
            return 1;
        }
        catch
        {
            return 4;
        }
    }


    private void PrintLanguageInfo(float maxWidth)
    {
        // Label
        BeginHorizontal();
        LabelField("Language file", EditorStyles.boldLabel, GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
        LabelField("Translate file", EditorStyles.boldLabel, GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
        LabelField("Language json", EditorStyles.boldLabel, GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
        EndHorizontal();

        // Elements
        for (LanguageType i = 0; i < LanguageType.End; ++i)
        {

            // UI language file
            string targetName = $"{i.ToString()}UI";
            BeginHorizontal();
            LabelField(targetName, GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
            PrintLanguageFileStatus(_curTras, targetName, maxWidth);
            PrintLanguageFileStatus(_curLans, targetName, maxWidth);
            EndHorizontal();

            // Runtime language file
            targetName = $"{i.ToString()}Runtime";
            BeginHorizontal();
            LabelField(targetName, GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
            PrintLanguageFileStatus(_curTras, targetName, maxWidth);
            PrintLanguageFileStatus(_curLans, targetName, maxWidth);
            EndHorizontal();
        }
    }


    private void PrintLanguageFileStatus(string[] targetArray, string targetname, float maxWidth)
    {
        if (targetname.Contains("Korean")
            && targetArray.Contains(targetname.Replace("Korean", "Translate")))
        {
            LabelField("Exist", GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
        }
        else if (targetArray.Contains(targetname))
        {
            LabelField("Exist", GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
        }
        else
        {
            EditorStyles.label.normal.textColor = new Color(1.0f, 0.3f, 0.3f, 1.0f);
            LabelField("Required", GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
            EditorStyles.label.normal.textColor = Color.white;
        }
    }


    private void SearchCurrentLanguageFiles()
    {
        // Translate files
        _languageData = new DirectoryInfo($"{Application.dataPath}/LanguageData").GetFiles("*.txt");
        _curTras = new string[_languageData.Length];
        for (byte i = 0; i < _languageData.Length; ++i)
        {
            _curTras[i] = _languageData[i].Name.Remove(_languageData[i].Name.IndexOf('.'));
        }

        // Language json
        FileInfo[] lanfiles = new DirectoryInfo($"{Application.dataPath}/Resources/Languages").GetFiles("*.json");
        _curLans = new string[lanfiles.Length];
        for (byte i = 0; i < lanfiles.Length; ++i)
        {
            _curLans[i] = lanfiles[i].Name.Remove(lanfiles[i].Name.IndexOf('.'));
        }
    }
}
