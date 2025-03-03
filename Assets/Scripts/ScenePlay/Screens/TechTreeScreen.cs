using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;

public class TechTreeScreen : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [SerializeField] private TextMeshProUGUI _title = null;
    [SerializeField] private TextMeshProUGUI _description = null;
    [SerializeField] private TextMeshProUGUI _income = null;
    [SerializeField] private TextMeshProUGUI _cost = null;
    [SerializeField] private TextMeshProUGUI _status = null;
    [SerializeField] private TextMeshProUGUI _adopt = null;
    [SerializeField] private TextMeshProUGUI _back = null;
    [SerializeField] private Image _adoptPregress = null;
    [SerializeField] private TechContentBase[] _contents = null;
    private TechTreeType _curCon = 0;
    private float _supportRate = 0.0f;
    private byte _curIndex = byte.MaxValue;
    private bool _adoptAvailable = false;
    private bool _backAvailable = true;

    static public TechTreeScreen Instance { get; private set; }
    public TechStatus[][] NodeStatus { get; set; }



    /* ==================== Public Methods ==================== */

    public void ButtonBack()
    {
        if (_backAvailable)
        {
            // Disables screen.
            _contents[(int)_curCon].SetActive(false);
            gameObject.SetActive(false);

            // Initialize
            SetAdoptAvailable(false);
            _contents[(int)_curCon].SetCursorPositoin(new Vector3(-10000.0f, -10000.0f, 0.0f));
            _curIndex = byte.MaxValue;
            _description.text = null;
            _income.text = null;
            _cost.text = null;
            _status.text = null;

            // Resume
            PlayManager.Instance.SetGamePause(false);

            // Audio play
            AudioManager.Instance.Play(AudioType.Touch);
        }
        else
        {
            // Audio play
            AudioManager.Instance.Play(AudioType.Unable);
        }
    }


    public void ButtonAdopt()
    {
        if (_adoptAvailable)
        {
            StartCoroutine(AdoptAnim());

            // Audio play
            AudioManager.Instance.Play(AudioType.Touch);
        }
        else
        {
            // Audio play
            AudioManager.Instance.Play(AudioType.Unable);
        }
    }


    /// <summary>
    /// Shows techtree screen.
    /// </summary>
    /// <param name="type">Content type to show.</param>
    /// <param name="supportRate">Decides success possibility.</param>
    /// <param name="title">Tite of the screen.</param>
    public void ShowScreen(TechTreeType type, float supportRate, string title)
    {
        // Screen set
        SetScreen(type, supportRate, title);

        if (type == TechTreeType.Facilities)
        {
            // Facilities null check
            NodeStatus[(int)TechTreeType.Facilities] = CitiesInfo.Instance.CurrentCity.Facilites;
            if (NodeStatus == null || NodeStatus.Length != _contents[(int)_curCon].NodeCount())
            {
                NodeStatus[(int)TechTreeType.Facilities] = new TechStatus[_contents[(int)_curCon].NodeCount()];
                CitiesInfo.Instance.CurrentCity.Facilites = NodeStatus[(int)TechTreeType.Facilities];
#if UNITY_EDITOR
                Debug.Log("Facility array has been newly created.");
#endif
            }
        }
        else
        {
            // Facilities not required.
            NodeStatus[(int)TechTreeType.Facilities] = null;
        }

        // Content show
        _contents[(int)_curCon].ShowContent(type);
    }


    /// <summary>
    /// Updates node info texts.
    /// </summary>
    /// <param name="index">Node index to show.</param>
    public void ShowNodeInfo(byte index)
    {
        if (_curIndex == index || !_backAvailable)
        {
            return;
        }
        _curIndex = index;

        // Texts update
        string[] infoText = new string[3];
        _contents[(int)_curCon].GetNodeInfoText(index, out infoText[0], out infoText[1], out infoText[2]);
        _description.text = infoText[0];
        _income.text = infoText[1];
        _cost.text = infoText[2];
        _status.text = null;

        SetAdoptAvailable(AvailableCheck());

        // Audio play
        AudioManager.Instance.Play(AudioType.Touch);
    }


    /// <summary>
    /// Sets cursor position.
    /// </summary>
    /// <param name="pos">Position where to move.</param>
    public void SetCursorPosition(Vector3 pos)
    {
        _contents[(int)_curCon].SetCursorPositoin(pos);
    }


    /// <summary>
    /// Get requirements array.
    /// </summary>
    /// <param name="type">Tech tree type of target.</param>
    /// <param name="index">Node index to get requirements.</param>
    /// <returns>Requirements</returns>
    public TechElement.ElementLink[] GetRequirements(TechTreeType type, byte index)
    {
        return _contents[(int)type].GetRequirements(index);
    }



    /* ==================== Private Methods ==================== */

    private void SetAdoptAvailable(bool available)
    {
        _adoptAvailable = available;
        if (available)
        {
            _adopt.color = Color.white;
        }
        else
        {
            _adopt.color = DISABLED;
        }
    }


    private void SetScreen(TechTreeType type, float supportRate, string title)
    {
        // Title
        _title.text = title;

        // Current content
        _curCon = type;

        // Support rate
        _supportRate = supportRate;

        // Screen active
        _contents[(int)_curCon].SetActive(true);
        gameObject.SetActive(true);
    }


    private bool AvailableCheck()
    {
        return (NodeStatus[(int)_curCon][_curIndex] == TechStatus.Available)
            && _contents[(int)_curCon].CostCheck(_curIndex);
    }


    private IEnumerator AdoptAnim()
    {
        // Cannot move
        SetAdoptAvailable(false);
        _backAvailable = false;
        _back.color = DISABLED;
        _status.text = null;

        // First cost
        _contents[(int)_curCon].FistCost(_curIndex);

        // Progress
        float time = 0.0f;
        do
        {
            time += Time.deltaTime;
            _adoptPregress.fillAmount = time / ADOPT_ANIM_TIME;
            yield return null;
        } while (time < ADOPT_ANIM_TIME);

        // Random success
        if (_supportRate >= Random.Range(0.0f, 100.0f))
        {
            // Element adopt
            _contents[(int)_curCon].Adopt(_curIndex);

            // Success text
            _status.text = LanguageManager.Instance[TEX_SUCCESS];
            _status.color = NORMALTEXT;

            // Audio play
            AudioManager.Instance.Play(AudioType.Select);
        }
        else
        {
            // Failed text
            _status.text = LanguageManager.Instance[TEX_FAILED];
            _status.color = FAILEDTEXT;

            // Audio play
            AudioManager.Instance.Play(AudioType.Failed);
        }

        // Coroutine end
        _adoptPregress.fillAmount = 0.0f;
        SetAdoptAvailable(AvailableCheck());
        _backAvailable = true;
        _back.color = Color.white;
    }


    private void Awake()
    {
        // Unity singleton pattern
        Instance = this;

        // Adoption data
        NodeStatus = new TechStatus[(int)TechTreeType.End][];

        // Screen initialize
        SetAdoptAvailable(false);
    }


    private void Start()
    {
        gameObject.SetActive(false);
    }
}
