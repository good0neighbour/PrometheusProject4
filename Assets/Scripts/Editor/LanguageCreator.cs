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
    private Dictionary<string, ushort> _fileLength = new Dictionary<string, ushort>();
    private List<string> _curRuns = null;
    private FileInfo[] _languageData = null;
    private string[] _curTras = null;
    private string[] _curLans = null;
    private string _textArea = null;
    private string _previousText = null;
    private byte _status = 0;
    private bool _curRunLanToggle = false;
    private bool _infoToggle = true;


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

        // New list
        if (_window._curRuns == null)
        {
            _window._curRuns = new List<string>();
        }

        _window.Show();
    }


    private void OnGUI()
    {
        LayoutUILanguage();
        Space(10.0f);
        LayoutRuntimeLanguage();

        // Runtime language info
        _curRunLanToggle = Foldout(_curRunLanToggle, "Current runtime language", true);
        PrintCurrentRuntimeLanguage();

        Space(10.0f);

        // Language file info
        _infoToggle = Foldout(_infoToggle, "Language file info", true);
        PrintLanguageInfo(120.0f);
    }


    private void LayoutUILanguage()
    {
        // Title
        LabelField("UI Language", EditorStyles.boldLabel);

        if (GUILayout.Button("Create UI language files from all scenes"))
        {
            string[] words = null;

            // File create
            _status = UIJsonCreate(out words);
            if (_status == 1)
            {
                _status = ForGoogleTranslate(words, "UI");
            }

            // Refresh
            AssetDatabase.Refresh();

            // Searches language files again.
            SearchCurrentLanguageFiles();
        }

        if (GUILayout.Button("Create Language Json based on translate files"))
        {
            // Creates json
            _status = TextToJson();

            // Refresh
            AssetDatabase.Refresh();

            // Searches language files again.
            SearchCurrentLanguageFiles();
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


    private byte ForGoogleTranslate(string[] words, string type)
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
            File.WriteAllText($"{Application.dataPath}/LanguageData/Translate{type}.txt", translateFile.ToString());

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

                    // Creates json.
                    LanguageManager.LanguageJson jsonData;
                    jsonData.Words = ReadTranslateFile(file.FullName);
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


    private void LayoutRuntimeLanguage()
    {
        // Title
        LabelField("Runtime Language", EditorStyles.boldLabel);

        // Input words
        _textArea = TextArea(_textArea);
        BeginHorizontal();
        if (GUILayout.Button("Add text"))
        {
            if (_curRuns.Contains(_textArea))
            {
                _status = 6;
            }
            else
            {
                _curRuns.Add(_textArea);
                _status = 5;
            }
            _previousText = _textArea;
            _textArea = null;
        }
        if (GUILayout.Button("Remove text"))
        {
            if (_curRuns.Contains(_textArea))
            {
                _curRuns.Remove(_textArea);
                _status = 7;
            }
            else
            {
                _status = 8;
            }
            _previousText = _textArea;
            _textArea = null;
        }
        EndHorizontal();

        // Modified text save
        if (GUILayout.Button("Create Runtime language files."))
        {
            RuntimeJsonCreate(_curRuns.ToArray());
            if (_status == 9)
            {
                _status = ForGoogleTranslate(_curRuns.ToArray(), "Runtime");
                if (_status == 3)
                {
                    _status = 11;
                }
            }

            // Refresh
            AssetDatabase.Refresh();

            // Searches language files again.
            SearchCurrentLanguageFiles();
        }

        // Print status
        switch (_status)
        {
            case 5:
                LabelField($"\"{_previousText}\" Successfully added.");
                break;
                
            case 6:
                LabelField($"\"{_previousText}\" Already exists.");
                break;
                
            case 7:
                LabelField($"\"{_previousText}\" Successfully removed.");
                break;
                
            case 8:
                LabelField($"\"{_previousText}\" Does not exists.");
                break;
                
            case 9:
                LabelField($"Successfully created.");
                break;
                
            case 10:
                LabelField($"Failed to create runtime language json.");
                break;
                
            case 11:
                LabelField($"Failed to create translate file.");
                break;

            default:
                LabelField("");
                break;
        }
    }


    private byte RuntimeJsonCreate(string[] wordsData)
    {
        try
        {
            // Words data
            LanguageManager.LanguageJson jsonData;
            jsonData.Words = wordsData;

            // Creates json file.
            File.WriteAllText(
                $"{Application.dataPath}/Resources/Languages/KoreanRuntime.json",
                JsonUtility.ToJson(jsonData, true)
            );

            // Successful
            return 9;
        }
        catch
        {
            return 10;
        }
    }


    private void PrintLanguageInfo(float maxWidth)
    {
        if (_infoToggle)
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
                BeginHorizontal();
                LabelField($"{i.ToString()}UI", GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
                PrintLanguageFileStatus(_curTras, i.ToString(), "UI.txt", maxWidth);
                PrintLanguageFileStatus(_curLans, i.ToString(), "UI.json", maxWidth);
                EndHorizontal();

                // Runtime language file
                BeginHorizontal();
                LabelField($"{i.ToString()}Runtime", GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
                PrintLanguageFileStatus(_curTras, i.ToString(), "Runtime.txt", maxWidth);
                PrintLanguageFileStatus(_curLans, i.ToString(), "Runtime.json", maxWidth);
                EndHorizontal();
            }
        }
    }


    private void PrintLanguageFileStatus(string[] targetArray, string targetname, string fileType, float maxWidth)
    {
        if (targetArray.Contains($"{targetname}{fileType}"))
        {
            if (_fileLength[$"{targetname}{fileType}"] == _fileLength[$"Korean{fileType}"])
            {
                LabelField("Exist", GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
            }
            else
            {
                EditorStyles.label.normal.textColor = new Color(1.0f, 0.3f, 0.3f, 1.0f);
                LabelField("Outdated", GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
                EditorStyles.label.normal.textColor = Color.white;
            }
        }
        else
        {
            EditorStyles.label.normal.textColor = new Color(1.0f, 0.3f, 0.3f, 1.0f);
            LabelField("Required", GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
            EditorStyles.label.normal.textColor = Color.white;
        }
    }


    private void PrintCurrentRuntimeLanguage()
    {
        if (_curRunLanToggle)
        {
            foreach (string word in _curRuns)
            {
                LabelField(word);
            }
        }
    }


    private void SearchCurrentLanguageFiles()
    {
        // Clears file lenghth dictionary
        _fileLength.Clear();

        // Translate files
        _languageData = new DirectoryInfo($"{Application.dataPath}/LanguageData").GetFiles("*.txt");
        _curTras = new string[_languageData.Length];
        for (byte i = 0; i < _languageData.Length; ++i)
        {
            // Loads current registered runtime words..
            if (_languageData[i].Name.Equals("KoreanRuntime.txt"))
            {
                _curRuns = ReadTranslateFile($"{Application.dataPath}/LanguageData/KoreanRuntime.txt").ToList();
            }

            // Records file type.
            if (_languageData[i].Name.Contains("Translate"))
            {
                _curTras[i] = _languageData[i].Name.Replace("Translate", "Korean");
            }
            else
            {
                _curTras[i] = _languageData[i].Name;
            }

            // Counts the number of words.
            _fileLength.Add(
                _curTras[i],
                (ushort)ReadTranslateFile(_languageData[i].FullName).Length
            );
        }

        // Language json
        FileInfo[] lanfiles = new DirectoryInfo($"{Application.dataPath}/Resources/Languages").GetFiles("*.json");
        _curLans = new string[lanfiles.Length];
        for (byte i = 0; i < lanfiles.Length; ++i)
        {
            // Records file type.
            _curLans[i] = lanfiles[i].Name;

            // Counts the number of words.
            _fileLength.Add(
                _curLans[i],
                (ushort)JsonUtility.FromJson<LanguageManager.LanguageJson>(
                    Resources.Load(
                        $"Languages/{_curLans[i].Remove(_curLans[i].IndexOf('.'))}"
                    ).ToString()
                ).Words.Length
            );
        }
    }


    private string[] ReadTranslateFile(string path)
    {
        // Text load
        string txt = File.ReadAllText(path);

        // Text trim
        if (!txt.EndsWith('\n'))
        {
            txt += '\n';
        }

        // Text save
        List<string> words = new List<string>();
        StringBuilder record = new StringBuilder();
        foreach (char ch in txt)
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

        // Retruen
        return words.ToArray();
    }
}
