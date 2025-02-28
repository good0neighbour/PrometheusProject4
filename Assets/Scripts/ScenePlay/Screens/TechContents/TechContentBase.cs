using UnityEngine;

abstract public class TechContentBase : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private RectTransform _cursor = null;
    [SerializeField] private Vector2 _nodeSize = new Vector2(120.0f, 120.0f);
    [SerializeField] private Vector2 _pivot = new Vector2(0.5f, 0.5f);
    private Node[] _nodes = null;



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// Shows available nodes and sets content size and pivot.
    /// </summary>
    public void ShowContent()
    {
        RectTransform rt = transform.parent.GetComponent<RectTransform>();

        // Sets content size.
        Vector2 contentSize = Vector2.zero;
        for (byte i = 0; i < _nodes.Length; ++i)
        {
            if (_nodes[i].IsShowed)
            {
                if (contentSize.x < _nodes[i].MaxPos.x)
                {
                    contentSize.x = _nodes[i].MaxPos.x;
                }
                if (contentSize.y < _nodes[i].MaxPos.y)
                {
                    contentSize.y = _nodes[i].MaxPos.y;
                }
            }
        }
        rt.sizeDelta = contentSize;

        // Sets content pivot.
        rt.pivot = _pivot;
    }


    public TechElement GetElement(byte index)
    {
        return _nodes[index].Data;
    }


    /// <summary>
    /// Sets cursor position.
    /// </summary>
    /// <param name="pos">Position where to move.</param>
    public void SetCursorPositoin(Vector3 pos)
    {
        _cursor.anchoredPosition = pos;
    }



    /* ==================== Protected Methods ==================== */

    abstract protected GameObject GetSampleNode();
    abstract protected TechTreeData GetTechTreeData();



    /* ==================== Private Methods ==================== */

    private void Start()
    {
        // Loads data.
        TechTreeData data = GetTechTreeData();

        // Node display
        _nodes = new Node[data.Elements.Length];
        for (byte i = 0; i < _nodes.Length; ++i)
        {
            // Creates a new node.
            _nodes[i] = new Node(
                Instantiate(GetSampleNode(), transform),
                data.Elements[i],
                _nodeSize
            );

            // Sets node transform.
            RectTransform node = _nodes[i].TechNode.GetComponent<RectTransform>();
            node.sizeDelta = _nodeSize;
            node.anchoredPosition = new Vector3(_nodes[i].MaxPos.x, _nodes[i].MaxPos.y, 0.0f);

            // Sets max position.
            _nodes[i].MaxPos.x += _nodeSize.x;
            _nodes[i].MaxPos.y += _nodeSize.y;

            // Sets node index.
            _nodes[i].TechNode.GetComponent<NodeButton>().Index = i;

            // Sets cursor size.
            _cursor.sizeDelta = _nodeSize;

            // Temporary
            _nodes[i].TechNode.SetActive(true);
        }
        // Temporary
        ShowContent();
    }



    /* ==================== Struct ==================== */

    protected struct Node
    {
        public GameObject TechNode;
        public TechElement Data;
        public Vector2 MaxPos;
        public bool IsShowed;

        public Node(GameObject techNode, TechElement data, Vector2 nodeSize)
        {
            TechNode = techNode;
            Data = data;
            MaxPos.x = data.XPos * nodeSize.x;
            MaxPos.y = data.YPos * nodeSize.y;
            // Temporary
            IsShowed = true;
        }
    }
}
