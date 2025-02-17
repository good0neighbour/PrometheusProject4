using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
using System.Text;

public class LanguageCreator : EditorWindow
{
    static private LanguageCreator _window = null;
    private byte _status = 0;


    [MenuItem("PrometheusMission/Languages", priority = 0)]
    static void Open()
    {
        if (null == _window)
        {
            _window = CreateInstance<LanguageCreator>();
            _window.position = new Rect(700.0f, 300.0f, 300.0f, 500.0f);
        }

        _window.ShowAuxWindow();
    }


    private void OnGUI()
    {
        if (GUILayout.Button("Create UI Language files from Scenes"))
        {
            string[] words = null;

            // File create
            _status = UIJsonCreate(out words);
            if (_status == 1)
            {
                _status = ForGoogleTranslate(words);
            }

            // Refresh
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Create Language Json based on translate files"))
        {
            // Creates json
            _status = TextToJson();

            // Refresh
            AssetDatabase.Refresh();
        }

            switch (_status)
        {
            case 1:
                GUILayout.Label("Successfully created.");
                return;

            case 2:
                GUILayout.Label("Failed to create UI Language json.");
                return;

            case 3:
                GUILayout.Label("Failed to create translate file.");
                return;

            case 4:
                GUILayout.Label("Failed to create language files.");
                return;

            default:
                return;
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
                translators = FindObjectsByType<LanguageTranslator>(FindObjectsSortMode.None);
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
            DirectoryInfo dI = new DirectoryInfo($"{Application.dataPath}/LanguageData");
            foreach (FileInfo file in dI.GetFiles("*.txt"))
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
            return 1;
        }
        catch
        {
            return 4;
        }
    }
}
