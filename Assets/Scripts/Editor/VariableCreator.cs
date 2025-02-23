using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

public class VariableCreator : EditorWindow
{
    [SerializeField] private List<VariableData> _data = null;
    static private VariableCreator _window = null;
    private SerializedObject _serializedObject = null;
    private SerializedProperty _property = null;
    private Vector2 scrollPos = Vector2.zero;
    private string _structureName = null;
    private string _fieldName = null;
    private byte _status = 0;


    [MenuItem("PrometheusMission/Variables", priority = 1)]
    static void Open()
    {
        if (null == _window)
        {
            _window = CreateInstance<VariableCreator>();
            _window.position = new Rect(700.0f, 300.0f, 400.0f, 500.0f);
        }

        // Loads variable data
        _window.LoadVariableData();

        _window._serializedObject = new SerializedObject(_window);
        _window._property = _window._serializedObject.FindProperty("_data");

        _window.Show();
    }


    private void OnGUI()
    {
        // Structure data
        _structureName = TextField("Structure Name", _structureName);
        _fieldName = TextField("Field Name", _fieldName);

        // Save button
        if (GUILayout.Button("Create varialbe script"))
        {
            _status = CreateJsonData();
            if (_status == 1)
            {
                _status = CreatScript();
            }
            AssetDatabase.Refresh();
        }

        // Status
        switch (_status)
        {
            case 1:
                Debug.Log("Successfully created the variable script.");
                _window.Close();
                return;

            case 2:
                LabelField("Failed to create json.");
                break;

            case 3:
                LabelField("Failed to create script.");
                break;

            default:
                LabelField("");
                break;
        }

        // Variable area
        scrollPos = BeginScrollView(scrollPos);

        _serializedObject.Update();
        PropertyField(_property, true, GUILayout.ExpandHeight(true));
        _serializedObject.ApplyModifiedProperties();

        EndScrollView();
    }


    private byte CreateJsonData()
    {
        try
        {
            File.WriteAllText($"{Application.dataPath}/Data/Variables/Variables.json", JsonUtility.ToJson(new VariableJson(_data.ToArray(), _structureName, _fieldName)).ToString());
            return 1;
        }
        catch
        {
            return 2;
        }
    }


    private byte CreatScript()
    {
        try
        {
            // Initial text
            StringBuilder builder = new StringBuilder(
                "public partial class PlayManager\n{"
            );
            builder.Append(
                "\n\t// Field"
            );
            builder.Append(
                $"\n\tprivate {_structureName} {_fieldName} = new {_structureName}();"
            );

            // Variables
            builder.Append(
                "\n\n\t// Properties"
            );
            for (ushort i = 0; i < _data.Count; ++i)
            {
                builder.Append(
                    $"\n\tpublic {_data[i].Type.ToString().ToLower()} {_data[i].Name}"
                );
                builder.Append(
                    $"{{ get {{ return _data.{_data[i].Name}; }} set {{ _data.{_data[i].Name} = value; }} }}"
                );
            }

            // Structure
            builder.Append(
                "\n\n\t// Structure"
            );
            builder.Append($"\n\tprivate struct {_structureName}\n\t{{");
            for (ushort i = 0; i < _data.Count; ++i)
            {
                builder.Append(
                    $"\n\t\tpublic {_data[i].Type.ToString().ToLower()} {_data[i].Name};"
                );
            }
            builder.Append("\n\t}");

            // End
            builder.Append("\n}");

            // Creates script.
            File.WriteAllText($"{Application.dataPath}/Data/Variables/PlayManager_variables.cs", builder.ToString());

            return 1;
        }
        catch
        {
            return 3;
        }
    }


    private void LoadVariableData()
    {
        try
        {
            VariableJson json = JsonUtility.FromJson<VariableJson>(File.ReadAllText($"{Application.dataPath}/Data/Variables/Variables.json"));
            _data = json.Variables.ToList();
            _structureName = json.StructureName;
            _fieldName = json.FieldName;
        }
        catch
        {
            _window._data = new List<VariableData>();
        }
    }


    private enum VarType
    {
        Byte,
        Short,
        Int,
        Long,
        Float,
        Double,
        SByte,
        UShort,
        UInt,
        ULong,
        UFloat,
        UDouble
    }


    [Serializable]
    private struct VariableData
    {
        public VarType Type;
        public string Name;

        public VariableData(VarType type, string name)
        {
            Type = type;
            Name = name;
        }
    }


    [Serializable]
    private struct VariableJson
    {
        public VariableData[] Variables;
        public string StructureName;
        public string FieldName;

        public VariableJson(VariableData[] variables, string structureName, string fieldName)
        {
            Variables = variables;
            StructureName = structureName;
            FieldName = fieldName;
        }
    }
}
