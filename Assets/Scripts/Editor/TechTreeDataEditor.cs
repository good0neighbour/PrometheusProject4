using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Unity.Android.Gradle;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(TechTreeData))]
public class TechTreeDataEditor : Editor
{
    private TechTreeData _instance = null;
    private List<List<TechElement.ElementLink>> _requirements = new List<List<TechElement.ElementLink>>();
    private List<TechElement> _elements = null;
    private List<bool> _toggles = new List<bool>();
    private StringBuilder _builder = new StringBuilder();
    private Vector2 _scrollPos = Vector2.zero;
    private float _nodeW = 30.0f;
    private float _nodeS = 20.0f;
    private byte _moveFrom = 0;
    private byte _moveTo = 0;
    private byte _addIndex = 0;


    private void OnEnable()
    {
        _instance = (TechTreeData)target;
        _elements = _instance.Elements.ToList();
        if (_elements == null)
        {
            _elements = new List<TechElement>();
        }

        for (byte i = 0; i < _elements.Count; ++i)
        {
            _requirements.Add(_elements[i].Requirements.ToList());
            _toggles.Add(false);
        }
    }


    public override void OnInspectorGUI()
    {
        _scrollPos = BeginScrollView(_scrollPos);

        for (byte i = 0; i < _elements.Count; ++i)
        {
            // Name
            if (_toggles[i])
            {
                _toggles[i] = Foldout(_toggles[i], $"Element {i.ToString()}\t{_elements[i].Name}", true);
                BeginHorizontal();
                Space(20.0f, false);
                LabelField("Name", GUILayout.MaxWidth(70.0f));
                EditorGUI.BeginChangeCheck();
                string name = TextField(_elements[i].Name, GUILayout.MinHeight(20.0f), GUILayout.MaxWidth(200.0f));
            }
            else
            {
                BeginHorizontal();
                _toggles[i] = Foldout(_toggles[i], $"Element {i.ToString()}\t{_elements[i].Name}", true);
                EditorGUI.BeginChangeCheck();
                string name = TextField(_elements[i].Name, GUILayout.MinHeight(20.0f));
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, $"TechElement name changed. {_elements[i].Name} => {name}");
                _elements[i].Name = name;
            }
            EndHorizontal();

            // Togle
            if (_toggles[i])
            {
                ToggleShow(i, 40.0f);
            }
        }

        EndScrollView();
        Space(30.0f, false);

        #region Move
        BeginHorizontal();
        LabelField("From", GUILayout.MaxWidth(35.0f));
        _moveFrom = (byte)IntField(_moveFrom, GUILayout.MaxWidth(50.0f));
        Space(10.0f, false);
        LabelField("To", GUILayout.MaxWidth(15.0f));
        _moveTo = (byte)IntField(_moveTo, GUILayout.MaxWidth(50.0f));
        Space(30.0f, false);
        if (GUILayout.Button("Move", GUILayout.MaxWidth(100.0f)))
        {
            Move();
        }
        EndHorizontal();
        #endregion
        #region Add, remove
        BeginHorizontal();
        LabelField("Index", GUILayout.MaxWidth(35.0f));
        _addIndex = (byte)IntField(_addIndex, GUILayout.MaxWidth(50.0f));
        Space(10.0f, false);
        if (GUILayout.Button("Add", GUILayout.MaxWidth(100.0f)))
        {
            Add();
        }
        if (GUILayout.Button("Remove", GUILayout.MaxWidth(100.0f)))
        {
            Remove();
        }
        EndHorizontal();
        #endregion
        ShowNodePositions();

        if (GUILayout.Button("Adopt all requirements from all tech tree data"))
        {
            ApplyAllRequirements();
        }
    }


    private byte GetIndex(TechTreeType type, string element)
    {
        TechElement[] data = AssetDatabase.LoadAssetAtPath<TechTreeData>($"Assets/Resources/TechTrees/{type.ToString()}.asset").Elements;
        for (byte i = 0; i < data.Length; ++i)
        {
            if (data[i].Name.Equals(element))
            {
                return i;
            }
        }
        return byte.MaxValue;
    }


    private void ToggleShow(byte i, float inputFieldWidth)
    {
        #region Description
        BeginHorizontal();
        Space(20.0f, false);
        LabelField("Description", GUILayout.MaxWidth(70.0f));
        EditorGUI.BeginChangeCheck();
        string des = TextField(_elements[i].Description, GUILayout.MinHeight(20.0f));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement {_elements[i].Name} description changed.");
            _elements[i].Description = des;
        }
        EndHorizontal();
        #endregion
        #region Costs
        BeginHorizontal();
        Space(20.0f, false);
        LabelField("Costs", GUILayout.MaxWidth(70.0f));

        // Fund cost
        EditorGUI.BeginChangeCheck();
        LabelField("Fund", GUILayout.MaxWidth(30.0f));
        float fundCost = FloatField(_elements[i].FundCost, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement FundCost changed. {_elements[i].FundCost.ToString()} => {fundCost.ToString()}");
            _elements[i].FundCost = fundCost;
        }

        Space(20.0f, false);

        // Maintenance
        EditorGUI.BeginChangeCheck();
        LabelField("Maintenance", GUILayout.MaxWidth(75.0f));
        float maintenance = FloatField(_elements[i].Maintenance, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement Maintenance changed. {_elements[i].Maintenance.ToString()} => {maintenance.ToString()}");
            _elements[i].Maintenance = maintenance;
        }

        Space(20.0f, false);

        // Research cost
        EditorGUI.BeginChangeCheck();
        LabelField("Research", GUILayout.MaxWidth(55.0f));
        float researchCost = FloatField(_elements[i].ResearchCost, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement ResearchCost changed. {_elements[i].ResearchCost.ToString()} => {researchCost.ToString()}");
            _elements[i].ResearchCost = researchCost;
        }

        Space(20.0f, false);

        // Culture cost
        EditorGUI.BeginChangeCheck();
        LabelField("Culture", GUILayout.MaxWidth(45.0f));
        float cultureCost = FloatField(_elements[i].CultureCost, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement CultureCost changed. {_elements[i].CultureCost.ToString()} => {cultureCost.ToString()}");
            _elements[i].CultureCost = cultureCost;
        }

        Space(20.0f, false);

        // Stone cost
        EditorGUI.BeginChangeCheck();
        LabelField("Stone", GUILayout.MaxWidth(35.0f));
        ushort stoneCost = (ushort)IntField(_elements[i].StoneCost, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement StoneCost changed. {_elements[i].StoneCost.ToString()} => {stoneCost.ToString()}");
            _elements[i].StoneCost = stoneCost;
        }

        Space(20.0f, false);

        // Iron cost
        EditorGUI.BeginChangeCheck();
        LabelField("Iron", GUILayout.MaxWidth(25.0f));
        ushort ironCost = (ushort)IntField(_elements[i].IronCost, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement IronCost changed. {_elements[i].IronCost.ToString()} => {ironCost.ToString()}");
            _elements[i].IronCost = ironCost;
        }

        Space(20.0f, false);

        // Nuclear cost
        EditorGUI.BeginChangeCheck();
        LabelField("Nuclear", GUILayout.MaxWidth(45.0f));
        ushort nuclearCost = (ushort)IntField(_elements[i].NuclearCost, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement IronCost changed. {_elements[i].NuclearCost.ToString()} => {nuclearCost.ToString()}");
            _elements[i].NuclearCost = nuclearCost;
        }

        EndHorizontal();
        #endregion
        #region Incomes
        BeginHorizontal();
        Space(20.0f, false);
        LabelField("Incomes", GUILayout.MaxWidth(70.0f));

        // Fund cost
        EditorGUI.BeginChangeCheck();
        LabelField("Electricity", GUILayout.MaxWidth(60.0f));
        ushort electricity = (ushort)IntField(_elements[i].Electricity, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement Electricity changed. {_elements[i].Electricity.ToString()} => {electricity.ToString()}");
            _elements[i].Electricity = electricity;
        }

        Space(20.0f, false);

        // Population
        EditorGUI.BeginChangeCheck();
        LabelField("Population", GUILayout.MaxWidth(65.0f));
        float population = FloatField(_elements[i].Population, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement Population changed. {_elements[i].Population.ToString()} => {population.ToString()}");
            _elements[i].Population = population;
        }
        EndHorizontal();
        #endregion
        #region Position
        BeginHorizontal();
        Space(20.0f, false);
        LabelField("Position", GUILayout.MaxWidth(70.0f));
        LabelField("X", GUILayout.MaxWidth(10.0f));
        EditorGUI.BeginChangeCheck();
        byte xPos = (byte)IntField(_elements[i].XPos, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement {_elements[i].Name} x position changed. {_elements[i].XPos} => {xPos}");
            _elements[i].XPos = xPos;
        }
        Space(20.0f, false);
        LabelField("Y", GUILayout.MaxWidth(10.0f));
        EditorGUI.BeginChangeCheck();
        byte yPos = (byte)IntField(_elements[i].YPos, GUILayout.MaxWidth(inputFieldWidth));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"TechElement {_elements[i].Name} y position changed. {_elements[i].YPos} => {yPos}");
            _elements[i].YPos = yPos;
        }
        EndHorizontal();
        #endregion
        #region Requirements
        for (byte j = 0; j < _requirements[i].Count; ++j)
        {
            BeginHorizontal();
            Space(20.0f, false);
            LabelField("Requries", GUILayout.MaxWidth(70.0f));

            // Type
            EditorGUI.BeginChangeCheck();
            TechTreeType type = (TechTreeType)EnumPopup(_requirements[i][j].Type, GUILayout.MaxWidth(100.0f));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, $"{_elements[i].Name} requirement type changed.");
                _requirements[i][j] = new TechElement.ElementLink(
                    type,
                    GetIndex(type, _requirements[i][j].Name),
                    _requirements[i][j].Name
                );
                _instance.Elements[i].Requirements[j] = _requirements[i][j];
            }

            // Index
            EditorGUI.BeginChangeCheck();
            string reqName = TextField(_requirements[i][j].Name, GUILayout.MinHeight(20.0f), GUILayout.MaxWidth(200.0f));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, $"{_elements[i].Name} requirement element changed.");
                _requirements[i][j] = new TechElement.ElementLink(
                    _requirements[i][j].Type,
                    GetIndex(_requirements[i][j].Type, reqName),
                    reqName
                );
                _instance.Elements[i].Requirements[j] = _requirements[i][j];
            }
            if (_requirements[i][j].Index == byte.MaxValue)
            {
                LabelField("Index not found");
            }
            else
            {
                LabelField($"Index: {_requirements[i][j].Index.ToString()}");
            }
            EndHorizontal();
        }

        // Add
        BeginHorizontal();
        Space(20.0f, false);
        if (GUILayout.Button("Add requirement"))
        {
            Undo.RecordObject(target, $"{_elements[i].Name} requirement added.");
            _requirements[i].Add(new TechElement.ElementLink());
            _elements[i].Requirements = _requirements[i].ToArray();
        }
        EndHorizontal();
        #endregion
        #region Texts
        BeginHorizontal();
        Space(20.0f, false);
        float height;
        LabelField(GetCostText(i, out height, 16.0f), GUILayout.MinHeight(height), GUILayout.MaxWidth(200.0f));
        LabelField(GetIncomeText(i, out height, 16.0f), GUILayout.MinHeight(height), GUILayout.MaxWidth(200.0f));
        LabelField(GetUnlockText(i, out height, 16.0f), GUILayout.MinHeight(height), GUILayout.MaxWidth(200.0f));
        LabelField($"[{_elements[i].Name}]\n{_elements[i].Description}", GUILayout.MinHeight(30.0f), GUILayout.MaxWidth(400.0f));
        EndHorizontal();
        #endregion
        Space(5.0f, false);
    }


    private void Move()
    {
        Undo.RecordObject(target, $"TechElement index moved. {_moveFrom} => {_moveTo}");
        TechElement element = _elements[_moveFrom];
        List<TechElement.ElementLink> reqEle = _requirements[_moveFrom];
        bool togle = _toggles[_moveFrom];
        _elements.RemoveAt(_moveFrom);
        _requirements.RemoveAt(_moveFrom);
        _toggles.RemoveAt(_moveFrom);
        _elements.Insert(_moveTo, element);
        _requirements.Insert(_moveTo, reqEle);
        _toggles.Insert(_moveTo, togle);
        _instance.Elements = _elements.ToArray();
    }


    private void Add()
    {
        if (_addIndex > _elements.Count)
        {
            _addIndex = (byte)_elements.Count;
        }
        Undo.RecordObject(target, $"TechElement added. Index: {_addIndex}");
        _elements.Insert(_addIndex, new TechElement());
        _requirements.Insert(_addIndex, new List<TechElement.ElementLink>());
        _toggles.Insert(_addIndex, true);
        _instance.Elements = _elements.ToArray();
        if (_addIndex >= _elements.Count - 1)
        {
            _addIndex = (byte)_elements.Count;
        }
    }


    private void Remove()
    {
        if (_addIndex >= _elements.Count)
        {
            _addIndex = (byte)(_elements.Count - 1);
        }
        Undo.RecordObject(target, $"TechElement removed. Index: {_addIndex}");
        _elements.RemoveAt(_addIndex);
        _requirements.RemoveAt(_addIndex);
        _toggles.RemoveAt(_addIndex);
        _instance.Elements = _elements.ToArray();
        if (_addIndex > _elements.Count)
        {
            _addIndex = (byte)_elements.Count;
        }
    }


    private string GetCostText(byte index, out float height, float lineHeight)
    {
        _builder.Clear();
        _builder.Append("[비용]");
        height = lineHeight;
        if (_elements[index].FundCost > 0.0f)
        {
            _builder.Append($"\n자금 {_elements[index].FundCost.ToString("0")}");
            height += lineHeight;
        }
        if (_elements[index].ResearchCost > 0.0f)
        {
            _builder.Append($"\n연구 {_elements[index].ResearchCost.ToString("0")}");
            height += lineHeight;
        }
        if (_elements[index].CultureCost > 0.0f)
        {
            _builder.Append($"\n문화 {_elements[index].CultureCost.ToString("0")}");
            height += lineHeight;
        }
        if (_elements[index].StoneCost > 0)
        {
            _builder.Append($"\n석제 {_elements[index].StoneCost.ToString()}");
            height += lineHeight;
        }
        if (_elements[index].IronCost > 0)
        {
            _builder.Append($"\n철 {_elements[index].IronCost.ToString()}");
            height += lineHeight;
        }
        if (_elements[index].NuclearCost > 0)
        {
            _builder.Append($"\n핵물질 {_elements[index].NuclearCost.ToString()}");
            height += lineHeight;
        }
        if (_elements[index].Maintenance > 0)
        {
            _builder.Append($"\n유지비용 {_elements[index].Maintenance.ToString()}");
            height += lineHeight;
        }
        return _builder.ToString();
    }


    private string GetIncomeText(byte index, out float height, float lineHeight)
    {
        _builder.Clear();
        _builder.Append("[수익]");
        height = lineHeight;
        if (_elements[index].Electricity > 0)
        {
            _builder.Append($"\n전력 {_elements[index].Electricity.ToString()}");
            height += lineHeight;
        }
        if (_elements[index].Population > 0.0f)
        {
            _builder.Append($"\n인구 변화 증가");
            height += lineHeight;
        }
        return _builder.ToString();
    }


    private string GetUnlockText(byte index, out float height, float lineHeight)
    {
        if (_elements[index].Unlocks == null)
        {
            height = 0.0f;
            return null;
        }
        _builder.Clear();
        _builder.Append("[잠금 해제]");
        height = lineHeight;
        for (int i = 0; i < _elements[index].Unlocks.Length; ++i)
        {
            _builder.Append($"\n{_elements[index].Unlocks[i].Name}");
            height += lineHeight;
        }
        return _builder.ToString();
    }


    private void ShowNodePositions()
    {
        if (_elements.Count == 0)
        {
            return;
        }
        byte[] xPoss = new byte[_elements.Count];
        byte[] yPoss = new byte[_elements.Count];
        byte maxX = _elements[0].XPos;
        byte maxY = _elements[0].YPos;
        byte minX = maxX;
        byte minY = maxY;
        for (byte i = 0; i < _elements.Count; ++i)
        {
            xPoss[i] = _elements[i].XPos;
            yPoss[i] = _elements[i].YPos;
            if (maxX < xPoss[i])
            {
                maxX = xPoss[i];
            }
            if (maxY < yPoss[i])
            {
                maxY = yPoss[i];
            }
            if (minX > xPoss[i])
            {
                minX = xPoss[i];
            }
            if (minY > yPoss[i])
            {
                minY = yPoss[i];
            }
        }

        maxX = (byte)(maxX - minX);
        maxY = (byte)(maxY - minY);
        string[,] map = new string[maxX + 1, maxY + 1];
        for (byte i = 0; i < _elements.Count; ++i)
        {
            map[xPoss[i] - minX, yPoss[i] - minY] = _elements[i].Name;
        }

        Space(20.0f, false);
        BeginHorizontal();
        LabelField("Node position", GUILayout.MaxWidth(80.0f));
        _nodeW = FloatField(_nodeW, GUILayout.MaxWidth(70.0f));
        _nodeS = FloatField(_nodeS, GUILayout.MaxWidth(70.0f));
        EndHorizontal();
        Space(_nodeS, false);
        for (sbyte i = (sbyte)maxY; i >= 0; --i)
        {
            BeginHorizontal();
            LabelField((minY + i).ToString(), GUILayout.MaxWidth(_nodeW * 0.5f));
            for (byte j = 0; j <= maxX; ++j)
            {
                if (string.IsNullOrEmpty(map[j, i]))
                {
                    LabelField("", GUILayout.MaxWidth(_nodeW));
                }
                else
                {
                    LabelField(map[j, i], GUILayout.MaxWidth(_nodeW));
                }
                Space(_nodeS, false);
            }
            EndHorizontal();
        }
        BeginHorizontal();
        LabelField("", GUILayout.MaxWidth(_nodeW * 0.5f));
        for (byte i = 0; i <= maxX; ++i)
        {
            Space(_nodeS, false);
            LabelField((minX + i).ToString(), GUILayout.MaxWidth(_nodeW));
        }
        EndHorizontal();
    }


    private void ApplyAllRequirements()
    {
        // Load
        TechTreeData[] techTrees = new TechTreeData[1];
        techTrees[0] = AssetDatabase.LoadAssetAtPath<TechTreeData>("Assets/Resources/TechTrees/Facilities.asset");

        // Unlocks list
        List<TechElement.ElementLink>[][] unlocks = new List<TechElement.ElementLink>[1][];
        for (byte i = 0; i < techTrees.Length; ++i)
        {
            unlocks[i] = new List<TechElement.ElementLink>[techTrees[i].Elements.Length];
            for (byte j = 0; j < techTrees[i].Elements.Length; ++j)
            {
                unlocks[i][j] = new List<TechElement.ElementLink>();
            }
        }

        // Unlocks set
        for (byte i = 0; i < techTrees.Length; ++i)
        {
            for (byte j = 0; j < techTrees[i].Elements.Length; ++j)
            {
                if (techTrees[i].Elements[j].Requirements == null)
                {
                    continue;
                }

                for (byte k = 0; k < techTrees[i].Elements[j].Requirements.Length; ++k)
                {
                    unlocks[(int)techTrees[i].Elements[j].Requirements[k].Type][techTrees[i].Elements[j].Requirements[k].Index].Add(
                        new TechElement.ElementLink(
                            (TechTreeType)i,
                            j,
                            techTrees[i].Elements[j].Name
                        )
                    );
                }
            }
        }

        // Unlocks adopt
        for (byte i = 0; i < techTrees.Length; ++i)
        {
            for (byte j = 0; j < techTrees[i].Elements.Length; ++j)
            {
                techTrees[i].Elements[j].Unlocks = unlocks[i][j].ToArray();
            }

            // Save
            EditorUtility.SetDirty(techTrees[i]);
        }
    }
}
