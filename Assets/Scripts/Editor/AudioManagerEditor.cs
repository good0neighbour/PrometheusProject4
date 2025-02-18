using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private AudioManager _instance = null;
    private AudioManager.AudioData[] _audioDatas = null;
    private bool[] _clipCheck = null;
    private bool _infoToggle = true;


    private void OnEnable()
    {
        _instance = (AudioManager)target;
        _audioDatas = _instance.AudioDatas;

        // Creates a new array
        if (_audioDatas == null)
        {
            _audioDatas = new AudioManager.AudioData[(int)AudioType.End];
            _instance.AudioDatas = _audioDatas;
        }
        else if (_audioDatas.Length < (int)AudioType.End)
        {
            AudioManager.AudioData[] newArray = new AudioManager.AudioData[(int)AudioType.End];
            for (byte i = 0; i < _audioDatas.Length; ++i)
            {
                newArray[i] = _audioDatas[i];
            }
            _audioDatas = newArray;
            _instance.AudioDatas = newArray;
        }
        else if (_audioDatas.Length > (int)AudioType.End)
        {
            AudioManager.AudioData[] newArray = new AudioManager.AudioData[(int)AudioType.End];
            for (byte i = 0; i < newArray.Length; ++i)
            {
                newArray[i] = _audioDatas[i];
            }
            _audioDatas = newArray;
            _instance.AudioDatas = newArray;
        }

        // All set to true.
        _clipCheck = new bool[(int)AudioType.End];
        for (byte i = 0; i < (int)AudioType.End; ++i)
        {
            _clipCheck[i] = true;
        }
    }


    public override void OnInspectorGUI()
    {
        // Number of audio channel
        EditorGUI.BeginChangeCheck();
        byte numOfChannel = (byte)IntField("Number of Audio Channels", _instance.NumOfChannel);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"Set the number of audio channel to {numOfChannel.ToString()}");
            _instance.NumOfChannel = numOfChannel;
        }

        Space(10.0f);
        LabelField("Audio Data", EditorStyles.boldLabel);
        Space(-10.0f);

        
        for (AudioType i = 0; i < AudioType.End; ++i)
        {
            // Audio clip null check
            if (_clipCheck[(int)i])
            {
                if (_audioDatas[(int)i].Clip == null)
                {
                    _audioDatas[(int)i].Clip = LoadAudioClip(i.ToString());
                    EditorUtility.SetDirty(_instance);
                }
                _clipCheck[(int)i] = false;
            }

            Space(10.0f);

            // Audio clip field
            EditorGUI.BeginChangeCheck();
            AudioClip clip = (AudioClip)ObjectField($"{i.ToString()} Clip", _audioDatas[(int)i].Clip, typeof(AudioClip), false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, $"Change audio clip of {i.ToString()}");
                _audioDatas[(int)i].Clip = clip;
            }

            // Volume field
            EditorGUI.BeginChangeCheck();
            float volume = FloatField($"{i.ToString()} Volume", _audioDatas[(int)i].Volume);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, $"Change audio volume of {i.ToString()} to {volume.ToString()}");
                _audioDatas[(int)i].Volume = volume;
            }
        }

        Space(10.0f);

        // Audio info
        _infoToggle = Foldout(_infoToggle, "Audio info", true);
        if (_infoToggle)
        {
            PrintAudioInfo();
        }
    }


    private AudioClip LoadAudioClip(string name)
    {
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/Sounds/{name}.wav");
        if (clip == null)
        {
            Debug.LogWarning($"\"{name}\" audio clip doesn't exist.");
            return null;
        }
        else
        {
            Debug.Log($"\"{name}\" audio clip is found.");
            return clip;
        }
    }


    private void PrintAudioInfo()
    {
        LabelField($"\tThe number of channels: {_instance.NumOfChannel}");
        LabelField($"\tThe number of audio data: {_instance.AudioDatas.Length}");
        for (AudioType i = 0; i < AudioType.End; ++i)
        {
            BeginHorizontal();
            LabelField($"\t\t{i.ToString()}", GUILayout.ExpandWidth(true), GUILayout.MaxWidth(120.0f), GUILayout.ExpandWidth(true));

            if (_instance.AudioDatas[(int)i].Clip == null)
                LabelField("Clip: null", GUILayout.MaxWidth(200.0f), GUILayout.ExpandWidth(true));
            else
                LabelField($"Clip: {_instance.AudioDatas[(int)i].Clip.ToString()}", GUILayout.MaxWidth(200.0f), GUILayout.ExpandWidth(true));

            if (_audioDatas[(int)i].Clip == null)
                LabelField("Clip: null", GUILayout.MaxWidth(250.0f), GUILayout.ExpandWidth(true));
            else
                LabelField($"Clip on editor: {_audioDatas[(int)i].Clip.ToString()}", GUILayout.MaxWidth(250.0f), GUILayout.ExpandWidth(true));

            LabelField($"Volume: {_instance.AudioDatas[(int)i].Volume.ToString()}", GUILayout.MaxWidth(70.0f), GUILayout.ExpandWidth(true));
            LabelField($"Volume on editor: {_audioDatas[(int)i].Volume.ToString()}", GUILayout.MaxWidth(100.0f), GUILayout.ExpandWidth(true));
            EndHorizontal();
        }
    }
}
