using System;
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
    [SerializeField] private List<RunLanData> _currentRuntimeLanguage = null;
    static private LanguageCreator _window = null;
    private Dictionary<string, ushort> _fileLength = new Dictionary<string, ushort>();
    private FileInfo[] _languageData = null;
    private string[] _curTras = null;
    private string[] _curLans = null;
    private string[] _curFons = null;
    private SerializedObject _serializedObject = null;
    private SerializedProperty _property = null;
    private Vector2 scrollPos = Vector2.zero;
    private string _searchText = null;
    private string _searchResu = null;
    private byte _status = 0;
    private bool _lanInfoToggle = true;
    private bool _fontInfoToggle = true;


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

        try
        {
            // Loads current registered runtime words.
            _window._currentRuntimeLanguage = JsonUtility.FromJson<RunLanJson>(File.ReadAllText($"{Application.dataPath}/Data/Variables/RuntimeLanguage.json")).Data.ToList();
        }
        catch
        {
            // New list
            if (_window._currentRuntimeLanguage == null)
            {
                _window._currentRuntimeLanguage = new List<RunLanData>();
            }
        }

        _window._serializedObject = new SerializedObject(_window);
        _window._property = _window._serializedObject.FindProperty("_currentRuntimeLanguage");

        _window.Show();
    }


    private void OnGUI()
    {
        // Button layout
        ButtonLayout();

        // Runtime language search
        LabelField("Runtime language search");
        BeginHorizontal();
        _searchText = TextField(_searchText);
        if (GUILayout.Button("Search", GUILayout.MaxWidth(100.0f)))
        {
            for (ushort i = 0; i < _currentRuntimeLanguage.Count; ++i)
            {
                _searchResu = "Doesn't exist.";
                if (_currentRuntimeLanguage[i].Text.Equals(_searchText))
                {
                    _searchResu = $"TEX_{_currentRuntimeLanguage[i].ConstName.ToUpper()}";
                    break;
                }
            }
        }
        EndHorizontal();
        LabelField(_searchResu);

        scrollPos = BeginScrollView(scrollPos);

        // Current runtime language
        _serializedObject.Update();
        PropertyField(_property, true);
        _serializedObject.ApplyModifiedProperties();

        // Language file info
        _lanInfoToggle = Foldout(_lanInfoToggle, "Language file info", true);
        PrintLanguageInfo(120.0f);

        Space(20.0f);

        // Font info
        _fontInfoToggle = Foldout(_fontInfoToggle, "Font info", true);
        PrintCurrentFont(120.0f);

        EndScrollView();
    }


    private void ButtonLayout()
    {
        // Finds all UI language from the scenes.
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

        // Current runtime language text save
        if (GUILayout.Button("Create Runtime language files with the texts"))
        {
            string[] textArray = null;
            _status = RunLanDataScriptCreate(out textArray);
            if (_status == 5)
            {
                _status = RuntimeJsonCreate(textArray);
                if (_status == 5)
                {
                    _status = ForGoogleTranslate(textArray, "Runtime");
                    if (_status == 3)
                    {
                        _status = 7;
                    }
                    else
                    {
                        _status = 5;
                    }
                }
            }

            // Refresh
            AssetDatabase.Refresh();

            // Searches language files again.
            SearchCurrentLanguageFiles();
        }

        // Creates json files from translate files.
        if (GUILayout.Button("Create all language json files based on translate files"))
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
                LabelField("Successfully created UI language.");
                break;

            case 2:
                LabelField("Failed to create UI Language json.");
                break;

            case 3:
                LabelField("Failed to create translate file.");
                break;

            case 4:
                LabelField("Failed to create language json files.");
                break;

            case 5:
                //LabelField($"Successfully created runtime language.");
                _window.Close();
                break;

            case 6:
                LabelField($"Failed to create runtime language json.");
                break;

            case 7:
                LabelField($"Failed to create translate file.");
                break;

            case 8:
                LabelField($"Failed to create constant script.");
                break;

            case 9:
                LabelField($"Successfully created language json files.");
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
            File.WriteAllText($"{Application.dataPath}/Data/Translates/Translate{type}.txt", translateFile.ToString());

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
                    if (file.Name.Equals("TranslateUI.txt") || file.Name.Equals("TranslateRuntime.txt"))
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
            return 9;
        }
        catch
        {
            return 4;
        }
    }


    private byte RunLanDataScriptCreate(out string[] textArray)
    {
        try
        {
            // Creates json file.
            File.WriteAllText(
                $"{Application.dataPath}/Data/Variables/RuntimeLanguage.json",
                JsonUtility.ToJson(new RunLanJson(_currentRuntimeLanguage.ToArray()), true)
            );

            // Builds constants script.
            StringBuilder builder = new StringBuilder("static public partial class Constants\n{");
            textArray = new string[_currentRuntimeLanguage.Count];
            for (ushort i = 0; i < _currentRuntimeLanguage.Count; ++i)
            {
                builder.Append($"\n\tpublic const string TEX_{_currentRuntimeLanguage[i].ConstName.ToUpper()} = \"{_currentRuntimeLanguage[i].Text}\";");

                // Text array
                textArray[i] = _currentRuntimeLanguage[i].Text;
            }
            builder.Append("\n}");

            // Creates constants script.
            File.WriteAllText($"{Application.dataPath}/Data/Variables/Constants_texts.cs", builder.ToString());

            return 5;
        }
        catch
        {
            textArray = null;
            return 8;
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
            return 5;
        }
        catch
        {
            return 6;
        }
    }


    private void PrintLanguageInfo(float maxWidth)
    {
        if (_lanInfoToggle)
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


    private void PrintCurrentFont(float maxWidth)
    {
        if (_fontInfoToggle)
        {
            // Label
            BeginHorizontal();
            LabelField("Font", EditorStyles.boldLabel, GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
            LabelField("Status", EditorStyles.boldLabel, GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
            EndHorizontal();

            // Elements
            for (LanguageType i = 0; i < LanguageType.End; ++i)
            {
                BeginHorizontal();
                LabelField(i.ToString(), GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
                if (_curFons.Contains($"Font{i.ToString()}"))
                {
                    LabelField("Exist", GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
                }
                else
                {
                    EditorStyles.label.normal.textColor = new Color(1.0f, 0.3f, 0.3f, 1.0f);
                    LabelField("Required", GUILayout.MaxWidth(maxWidth), GUILayout.ExpandWidth(false));
                    EditorStyles.label.normal.textColor = Color.white;
                }
                EndHorizontal();
            }
        }
    }


    private void SearchCurrentLanguageFiles()
    {
        // Clears file lenghth dictionary
        _fileLength.Clear();

        // Translate files
        _languageData = new DirectoryInfo($"{Application.dataPath}/Data/Translates").GetFiles("*.txt");
        _curTras = new string[_languageData.Length];
        for (byte i = 0; i < _languageData.Length; ++i)
        {
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

        // Font
        lanfiles = new DirectoryInfo($"{Application.dataPath}/Resources/Fonts").GetFiles("*.asset");
        _curFons = new string[lanfiles.Length];
        for (byte i = 0; i < lanfiles.Length; ++i)
        {
            // Records file type.
            _curFons[i] = lanfiles[i].Name.Remove(lanfiles[i].Name.IndexOf('.'));
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


    [Serializable]
    private struct RunLanData
    {
        public string Text;
        public string ConstName;
    }


    [Serializable]
    private struct RunLanJson
    {
        public RunLanData[] Data;

        public RunLanJson(RunLanData[] data)
        {
            Data = data;
        }
    }
}
