using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(TechTreeData))]
public class TechTreeDataEditor : Editor
{
    private TechTreeData _instance = null;
    private List<List<RequirementData>> _requirements = null;
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
        _addIndex = (byte)_elements.Count;
        _requirements = new List<List<RequirementData>>();
        for (byte i = 0; i < _addIndex; ++i)
        {
            _requirements.Add(new List<RequirementData>());
            for (byte j = 0; j < _elements[i].Requirements.Length; ++j)
            {
                _requirements[i].Add(new RequirementData(
                    _elements[i].Requirements[j],
                    GetName(_elements[i].Requirements[j])
                ));
            }
            _toggles.Add(false);
        }
    }


    public override void OnInspectorGUI()
    {
        _scrollPos = BeginScrollView(_scrollPos);

        for (byte i = 0; i < _elements.Count; ++i)
        {
            // Name
            BeginHorizontal();
            _toggles[i] = Foldout(_toggles[i], $"Element {i.ToString()}\t{_elements[i].Name}", true);
            EditorGUI.BeginChangeCheck();
            string name = TextField(_elements[i].Name, GUILayout.MinHeight(20.0f));
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
        Space(30.0f);

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

        #region Node positoin
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

        Space(20.0f);
        BeginHorizontal();
        LabelField("Node position", GUILayout.MaxWidth(80.0f));
        _nodeW = FloatField(_nodeW, GUILayout.MaxWidth(70.0f));
        _nodeS = FloatField(_nodeS, GUILayout.MaxWidth(70.0f));
        EndHorizontal();
        Space(_nodeS);
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
        #endregion
    }


    private string GetName(TechElement.Requirement requirement)
    {
        try
        {
            return AssetDatabase.LoadAssetAtPath<TechTreeData>($"Assets/Data/TechTrees/{requirement.Type.ToString()}.asset").Elements[requirement.Index].Name;
        }
        catch
        {
            return null;
        }
    }


    private byte GetIndex(TechTreeType type, string element)
    {
        try
        {
            TechElement[] data = AssetDatabase.LoadAssetAtPath<TechTreeData>($"Assets/Data/TechTrees/{type.ToString()}.asset").Elements;
            for (byte i = 0; i < data.Length; ++i)
            {
                if (data[i].Name.Equals(element))
                {
                    return i;
                }
            }
            return 0;
        }
        catch
        {
            return 0;
        }
    }


    private void ToggleShow(byte i, float inputFieldWidth)
    {
        Space(20.0f);
        #region Description
        BeginHorizontal();
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
            LabelField("Requries", GUILayout.MaxWidth(70.0f));

            // Type
            EditorGUI.BeginChangeCheck();
            TechTreeType type = (TechTreeType)EnumPopup(_requirements[i][j].Requirement.Type, GUILayout.MaxWidth(100.0f));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, $"{_elements[i].Name} requirement type changed.");
                _requirements[i][j] = new RequirementData(
                    new TechElement.Requirement(type, _requirements[i][j].Requirement.Index),
                    GetName(_requirements[i][j].Requirement)
                );
                _instance.Elements[i].Requirements[j] = _requirements[i][j].Requirement;
            }

            // Index
            EditorGUI.BeginChangeCheck();
            string reqName = TextField(_requirements[i][j].Name, GUILayout.MaxWidth(200.0f));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, $"{_elements[i].Name} requirement element changed.");
                _requirements[i][j] = new RequirementData(
                    new TechElement.Requirement(
                        _requirements[i][j].Requirement.Type,
                        GetIndex(_requirements[i][j].Requirement.Type, reqName)
                    ),
                    reqName
                );
                _instance.Elements[i].Requirements[j] = _requirements[i][j].Requirement;
            }
            LabelField($"Index: {_requirements[i][j].Requirement.Index.ToString()}");
            EndHorizontal();
        }

        // Add
        if (GUILayout.Button("Add requirement"))
        {
            Undo.RecordObject(target, $"{_elements[i].Name} requirement added.");
            _requirements[i].Add(new RequirementData());
            List<TechElement.Requirement> newList = new List<TechElement.Requirement>();
            for (byte j = 0; j < _requirements[i].Count; ++j)
            {
                newList.Add(_requirements[i][j].Requirement);
            }
            _elements[i].Requirements = newList.ToArray();
        }
        #endregion
        #region Texts
        BeginHorizontal();
        float height;
        LabelField(GetCostText(i, out height, 16.0f), GUILayout.MinHeight(height), GUILayout.MaxWidth(200.0f));
        LabelField(GetIncomeText(i, out height, 16.0f), GUILayout.MinHeight(height), GUILayout.MaxWidth(200.0f));
        LabelField($"[{_elements[i].Name}]\n{_elements[i].Description}", GUILayout.MinHeight(30.0f));
        EndHorizontal();
        #endregion
        Space(30.0f);
    }


    private void Move()
    {
        Undo.RecordObject(target, $"TechElement index moved. {_moveFrom} => {_moveTo}");
        TechElement element = _elements[_moveFrom];
        List<RequirementData> reqEle = _requirements[_moveFrom];
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
        _requirements.Insert(_addIndex, new List<RequirementData>());
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


    private struct RequirementData
    {
        public TechElement.Requirement Requirement;
        public string Name;

        public RequirementData(TechElement.Requirement requirement, string name)
        {
            Requirement = requirement;
            Name = name;
        }
    }
}
