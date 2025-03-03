using System.Text;
using UnityEngine;
using static Constants;

abstract public class TechContentBase : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private RectTransform _cursor = null;
    [SerializeField] private Vector2 _nodeSize = new Vector2(120.0f, 120.0f);
    [SerializeField] private Vector2 _pivot = new Vector2(0.5f, 0.5f);
    protected Node[] Nodes = null;
    protected Vector2 ContentSize = new Vector2();



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// Shows available nodes and sets content size and pivot.
    /// </summary>
    public void ShowContent(TechTreeType type)
    {
        RectTransform rt = transform.parent.GetComponent<RectTransform>();
        TechStatus[][] status = TechTreeScreen.Instance.NodeStatus;
        ContentSize = Vector2.zero;

        for (byte i = 0; i < Nodes.Length; ++i)
        {
            // Requirements check
            switch (status[(int)type][i])
            {
                case TechStatus.Unavailable:
                    if (RequirementsCheck(Nodes[i].Data.Requirements))
                    {
                        status[(int)type][i] = TechStatus.Available;
                        GetMaxSize(Nodes[i].MaxPos);
                    }
                        break;

                default:
                    GetMaxSize(Nodes[i].MaxPos);
                    break;
            }

            // Node status
            Nodes[i].TechNode.SetNodeStatus(status[(int)type][i]);
        }

        // Sets content size.
        rt.sizeDelta = ContentSize;

        // Sets content pivot.
        rt.pivot = _pivot;
    }


    /// <summary>
    /// Sets cursor position.
    /// </summary>
    /// <param name="pos">Position where to move.</param>
    public void SetCursorPositoin(Vector3 pos)
    {
        _cursor.anchoredPosition = pos;
    }


    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }


    /// <summary>
    /// Gets node information texts.
    /// </summary>
    /// <param name="index">Node index</param>
    /// <param name="description">Description text</param>
    /// <param name="income">Income text</param>
    /// <param name="cost">Cost text</param>
    public void GetNodeInfoText(byte index, out string description, out string income, out string cost)
    {
        TechElement element = Nodes[index].Data;
        description = $"[{element.Name}]\n{element.Description}";
        income = GetIncomeText(element);
        cost = GetCostText(element);
    }


    /// <summary>
    /// Gets the number of nodes.
    /// </summary>
    /// <returns>The number of nodes.</returns>
    public byte NodeCount()
    {
        return (byte)Nodes.Length;
    }


    /// <summary>
    /// Gets requirment array.
    /// </summary>
    /// <param name="index">Node index to get requirements.</param>
    /// <returns>Requirments</returns>
    public TechElement.ElementLink[] GetRequirements(byte index)
    {
        return Nodes[index].Data.Requirements;
    }


    /// <summary>
    /// Pay the cost as soon as pressing adopt button.
    /// </summary>
    /// <param name="index">Current node index.</param>
    public void FistCost(byte index)
    {
        TechElement element = Nodes[index].Data;

        PlayManager.Instance.Fund -= element.FundCost;
        PlayManager.Instance.Research -= element.ResearchCost;
        PlayManager.Instance.Culture -= element.CultureCost;
        PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Fund);
        PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Research);
        PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Culture);
    }


    /// <summary>
    /// Checks cost available.
    /// </summary>
    /// <param name="index">Node index to check.</param>
    /// <returns>Available or not.</returns>
    public bool CostCheck(byte index)
    {
        TechElement element = Nodes[index].Data;

        // Cost check
        if (element.FundCost > PlayManager.Instance.Fund)
        {
            return false;
        }
        if (element.ResearchCost > PlayManager.Instance.Research)
        {
            return false;
        }
        if (element.CultureCost > PlayManager.Instance.Culture)
        {
            return false;
        }
        if (element.StoneCost > PlayManager.Instance.Stone)
        {
            return false;
        }
        if (element.IronCost > PlayManager.Instance.Iron)
        {
            return false;
        }
        if (element.NuclearCost > PlayManager.Instance.Nuclear)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Adopts tech node.
    /// </summary>
    /// <param name="index">Node index to adopt.</param>
    virtual public void Adopt(byte index)
    {
        TechElement element = Nodes[index].Data;
        Nodes[index].TechNode.SetNodeStatus(TechStatus.Adopted);

        // Cost
        if (element.StoneCost > 0)
        {
            PlayManager.Instance.StoneUsage += element.StoneCost;
            PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Stone);
        }
        if (element.IronCost > 0)
        {
            PlayManager.Instance.IronUsage += element.IronCost;
            PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Iron);
        }
        if (element.NuclearCost > 0)
        {
            PlayManager.Instance.NuclearUsage += element.NuclearCost;
            PlayMenuManager.Instance.BottomInfoUpdate(BottomInfoType.Nuclear);
        }

        // Income
        if (element.Electricity > 0)
        {
            PlayManager.Instance.ElectricityTotal += element.Electricity;
        }

        // Unlock check
        UnlockCheck(element.Unlocks, Nodes[index].MaxPos);
    }



    /* ==================== Protected Methods ==================== */

    protected bool RequirementsCheck(TechElement.ElementLink[] requirements)
    {
        // No requirements
        if (requirements == null)
        {
            return true;
        }

        // Check
        TechStatus[][] status = TechTreeScreen.Instance.NodeStatus;
        for (int i = 0; i < requirements.Length; ++i)
        {
            if (status[(int)requirements[i].Type][requirements[i].Index]
                == TechStatus.Adopted)
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        return true;
    }


    protected void GetMaxSize(Vector2 newPos)
    {
        if (ContentSize.x < newPos.x)
        {
            ContentSize.x = newPos.x;
        }
        if (ContentSize.y < newPos.y)
        {
            ContentSize.y = newPos.y;
        }
    }


    abstract protected void UnlockCheck(TechElement.ElementLink[] unlocks, Vector2 nodePos);
    abstract protected GameObject GetSampleNode();
    abstract protected TechTreeData GetTechTreeData();



    /* ==================== Private Methods ==================== */

    private string GetCostText(TechElement element)
    {
        StringBuilder builder = GameManager.Instance.Builder;
        builder.Clear();
        builder.Append($"[{LanguageManager.Instance[TEX_COST]}]");
        if (element.FundCost > 0.0f)
        {
            builder.Append($"\n{LanguageManager.Instance[TEX_FUND]} {element.FundCost.ToString("0")}");
        }
        if (element.ResearchCost > 0.0f)
        {
            builder.Append($"\n{LanguageManager.Instance[TEX_RESEARCH]} {element.ResearchCost.ToString("0")}");
        }
        if (element.CultureCost > 0.0f)
        {
            builder.Append($"\n{LanguageManager.Instance[TEX_CULTURE]} {element.CultureCost.ToString("0")}");
        }
        if (element.StoneCost > 0)
        {
            builder.Append($"\n{LanguageManager.Instance[TEX_STONE]} {element.StoneCost.ToString()}");
        }
        if (element.IronCost > 0)
        {
            builder.Append($"\n{LanguageManager.Instance[TEX_IRON]} {element.IronCost.ToString()}");
        }
        if (element.NuclearCost > 0)
        {
            builder.Append($"\n{LanguageManager.Instance[TEX_NUCLEAR]} {element.NuclearCost.ToString()}");
        }
        if (element.Maintenance > 0)
        {
            builder.Append($"\n{LanguageManager.Instance[TEX_MAINTENANCE]} {element.Maintenance.ToString()}");
        }
        return builder.ToString();
    }


    private string GetIncomeText(TechElement element)
    {
        StringBuilder builder = GameManager.Instance.Builder;
        builder.Clear();
        builder.Append($"[{LanguageManager.Instance[TEX_INCOME]}]");
        if (element.Electricity > 0)
        {
            builder.Append($"\n{LanguageManager.Instance[TEX_ELECTRICITY]} {element.Electricity.ToString()}");
        }
        if (element.Population > 0.0f)
        {
            builder.Append($"\n{LanguageManager.Instance[TEX_POPMOV]}");
        }
        return builder.ToString();
    }


    private void Start()
    {
        // Loads data.
        TechTreeData data = GetTechTreeData();

        // Node display
        Nodes = new Node[data.Elements.Length];
        for (byte i = 0; i < Nodes.Length; ++i)
        {
            // Creates a new node.
            Nodes[i] = new Node(
                Instantiate(GetSampleNode(), transform).GetComponent<NodeButton>(),
                data.Elements[i],
                _nodeSize
            );

            // Sets node index.
            Nodes[i].TechNode.Index = i;

            // Sets node transform.
            RectTransform node = Nodes[i].TechNode.GetComponent<RectTransform>();
            node.sizeDelta = _nodeSize;
            node.anchoredPosition = new Vector3(Nodes[i].MaxPos.x, Nodes[i].MaxPos.y, 0.0f);

            // Sets max position.
            Nodes[i].MaxPos.x += _nodeSize.x;
            Nodes[i].MaxPos.y += _nodeSize.y;

            // Sets cursor size.
            _cursor.sizeDelta = _nodeSize;
        }

        gameObject.SetActive(false);
    }



    /* ==================== Struct ==================== */

    protected struct Node
    {
        public NodeButton TechNode;
        public TechElement Data;
        public Vector2 MaxPos;

        public Node(NodeButton techNode, TechElement data, Vector2 nodeSize)
        {
            TechNode = techNode;
            Data = data;
            MaxPos.x = data.XPos * nodeSize.x;
            MaxPos.y = data.YPos * nodeSize.y;
        }
    }
}
