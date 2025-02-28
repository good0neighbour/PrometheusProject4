using System.Text;
using UnityEngine;
using TMPro;

public class TechTreeScreen : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private TextMeshProUGUI _description = null;
    [SerializeField] private TextMeshProUGUI _income = null;
    [SerializeField] private TextMeshProUGUI _cost = null;
    [SerializeField] private TextMeshProUGUI _status = null;
    [SerializeField] private TechContentBase[] _contents = null;
    private StringBuilder _builder = new StringBuilder();
    private byte _curCon = 0;
    private byte _curIndex = byte.MaxValue;

    static public TechTreeScreen Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonBack()
    {
        // Disables screen.
        gameObject.SetActive(false);

        // Initialize
        _contents[_curCon].SetCursorPositoin(new Vector3(-10000.0f, -10000.0f, 0.0f));
        _curIndex = byte.MaxValue;
        _description.text = null;
        _income.text = null;
        _cost.text = null;
        _status.text = null;

        // Audio play
        AudioManager.Instance.Play(AudioType.Touch);
    }


    /// <summary>
    /// Shows techtree screen.
    /// </summary>
    /// <param name="type">Content type to show.</param>
    public void ShowScreen(TechTreeType type)
    {
        // Current content
        _curCon = (byte)type;

        // Content show
        _contents[_curCon].ShowContent();

        // Screen active
        gameObject.SetActive(true);
    }


    /// <summary>
    /// Updates node info texts.
    /// </summary>
    /// <param name="index">Node index to show.</param>
    public void ShowNodeInfo(byte index)
    {
        if (_curIndex == index)
        {
            return;
        }
        _curIndex = index;
        
        // Texts update
        TechElement element = _contents[_curCon].GetElement(index);
        _description.text = $"[{element.Name}]\n{element.Description}";
        _income.text = GetIncomeText(element);
        _cost.text = GetCostText(element);

        // Audio play
        AudioManager.Instance.Play(AudioType.Touch);
    }


    /// <summary>
    /// Sets cursor position.
    /// </summary>
    /// <param name="pos">Position where to move.</param>
    public void SetCursorPosition(Vector3 pos)
    {
        _contents[_curCon].SetCursorPositoin(pos);
    }



    /* ==================== Private Methods ==================== */

    private string GetIncomeText(TechElement element)
    {
        _builder.Clear();
        _builder.Append("[수익]");
        if (element.Electricity > 0)
        {
            _builder.Append($"\n전력 {element.Electricity.ToString()}");
        }
        if (element.Population > 0.0f)
        {
            _builder.Append($"\n인구 변화 증가");
        }
        return _builder.ToString();
    }


    private string GetCostText(TechElement element)
    {
        _builder.Clear();
        _builder.Append("[비용]");
        if (element.FundCost > 0.0f)
        {
            _builder.Append($"\n자금 {element.FundCost.ToString("0")}");
        }
        if (element.ResearchCost > 0.0f)
        {
            _builder.Append($"\n연구 {element.ResearchCost.ToString("0")}");
        }
        if (element.CultureCost > 0.0f)
        {
            _builder.Append($"\n문화 {element.CultureCost.ToString("0")}");
        }
        if (element.StoneCost > 0)
        {
            _builder.Append($"\n석제 {element.StoneCost.ToString()}");
        }
        if (element.IronCost > 0)
        {
            _builder.Append($"\n철 {element.IronCost.ToString()}");
        }
        if (element.NuclearCost > 0)
        {
            _builder.Append($"\n핵물질 {element.NuclearCost.ToString()}");
        }
        if (element.Maintenance > 0)
        {
            _builder.Append($"\n유지비용 {element.Maintenance.ToString()}");
        }
        return _builder.ToString();
    }


    private void Awake()
    {
        // Unity singleton pattern
        Instance = this;
    }
}
