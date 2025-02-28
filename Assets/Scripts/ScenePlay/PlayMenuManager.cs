using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;

public class PlayMenuManager : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [Header("Section Buttons")]
    [SerializeField] private GameObject _buttonLeft = null;
    [SerializeField] private GameObject _buttonRight = null;
    [Header("Play Menu")]
    [SerializeField] private GameObject[] _MenuButtons = new GameObject[2];
    [SerializeField] private GameObject[] _PlayMenus = new GameObject[3];
    [Header("Game Speed Controls")]
    [SerializeField] private TextMeshProUGUI _pauseButtonText = null;
    [SerializeField] private TextMeshProUGUI _speedButtonText = null;
    [SerializeField] private Color _activeColour = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    [SerializeField] private Color _deactiveColour = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    [Header("Date")]
    [SerializeField] private TextMeshProUGUI _dateText = null;
    [Header("Noise Material")]
    [SerializeField][Range(0.0f, 1.0f)] private float _normalBrightness = 0.3921568627450980392156862745098f;
    [SerializeField][Range(0.0f, 1.0f)] private float _interactionBrightness = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float _noiseAlpha = 0.94117647058823529411764705882353f;
    [SerializeField] private float _noiseFadeOutTime = 0.5f;
    [SerializeField] private Image[] _noiseBackgrounds = null;
    [SerializeField] private Material _noiseMaterial = null;
    [Header("Camera Position")]
    [SerializeField] private float _camSidePos = 1.0f;
    [SerializeField] private float _camMoveTime = 2.0f;
    [SerializeField] private float _camMovePow = 2.0f;
    [Header("Bottom")]
    [SerializeField] private TextMeshProUGUI _fundText = null;
    [SerializeField] private TextMeshProUGUI _fundAmount = null;
    private StringBuilder _speedTextBuilder = new StringBuilder();
    private Coroutine _noiseCoroutine = null;
    private Coroutine _cameraCoroutine = null;
    private sbyte _currentSection = 0;
    private bool _isPaused = false;

    public static PlayMenuManager Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonSection(int direction)
    {
        // Section move
        _currentSection += (sbyte)direction;

        // Section active
        switch (_currentSection)
        {
            case -1:
                SectionActive(true, false);
                break;

            case 0:
                SectionActive(false, false);
                break;

            case 1:
                SectionActive(false, true);
                break;

            default:
#if UNITY_EDITOR
                Debug.LogError($"Wrong section number.\n{name}, PlayMenuManager, _currentSection : {_currentSection.ToString()}");
                _currentSection -= (sbyte)direction;
#endif
                break;
        }

        // Audio play
        AudioManager.Instance.Play(AudioType.Touch);

        // Noise brightness change
        if (_noiseCoroutine != null)
        {
            StopCoroutine(_noiseCoroutine);
        }
        _noiseCoroutine = StartCoroutine(NoiseBrightnessAnim());

        // Camera Move
        if (_cameraCoroutine != null)
        {
            StopCoroutine(_cameraCoroutine);
        }
        _cameraCoroutine = StartCoroutine(SectionMoveAnim(_camSidePos * _currentSection));

        // Message box move
        MessageManager.Instance.StartMesBoxMoveCoroutine(_currentSection);
    }


    public void ButtonGameSpeed()
    {
        if (_isPaused)
        {
            // Resumes game when the game is paused.
            ButtonGamePause();
        }
        else
        {
            // Game speed set and sprite change
            byte gameSpeed = PlayManager.Instance.SetGameSpeed();
            _speedTextBuilder.Clear();
            for (byte i = 0; i < gameSpeed; ++i)
            {
                _speedTextBuilder.Append('4');
            }
            _speedButtonText.text = _speedTextBuilder.ToString();

            // Audio play
            AudioManager.Instance.Play(AudioType.Touch);
        }
    }


    public void ButtonGamePause()
    {
        if (_isPaused)
        {
            PlayManager.Instance.SetGamePause(false);
            _pauseButtonText.color = _deactiveColour;
            _speedButtonText.color = _activeColour;
            _isPaused = false;
        }
        else
        {
            PlayManager.Instance.SetGamePause(true);
            _pauseButtonText.color = _activeColour;
            _speedButtonText.color = _deactiveColour;
            _isPaused = true;
        }

        // Audio play
        AudioManager.Instance.Play(AudioType.Touch);
    }


    /// <summary>
    /// Enables selected menu and disables previous menu.
    /// </summary>
    /// <param name="isLeft">True when it is left menu.</param>
    /// <param name="selectedMenu">Menu which is selected.</param>
    public void MenuMove(bool isLeft, GameObject selectedMenu)
    {
        if (isLeft)
        {
            // Checks if it is already selected.
            if (_PlayMenus[1] == selectedMenu)
            {
                return;
            }

            // Left menu switch
            _PlayMenus[1].SetActive(false);
            _PlayMenus[1] = selectedMenu;
            _PlayMenus[1].SetActive(true);
        }
        else
        {
            // Checks if it is already selected.
            if (_PlayMenus[2] == selectedMenu)
            {
                return;
            }

            // Right menu switch
            _PlayMenus[2].SetActive(false);
            _PlayMenus[2] = selectedMenu;
            _PlayMenus[2].SetActive(true);
        }

        // Audio play
        AudioManager.Instance.Play(AudioType.Touch);

        // Noise brightness change
        StopAllCoroutines();
        StartCoroutine(NoiseBrightnessAnim());
    }


    public void FundUpdate()
    {
        _fundAmount.text = Mathf.RoundToInt(PlayManager.Instance.Fund).ToString();
        StartCoroutine(CostAnim(_fundText, _fundAmount));
    }



    /* ==================== Private Methods ==================== */

    private void SectionActive(bool left, bool right)
    {
#if UNITY_EDITOR
        if (left && right)
        {
            Debug.LogError($"Left, right sections cannot be true at the same time.\n{name}, PlayManager, SectionActive(bool left, bool right), left: {left.ToString()}, right: {right.ToString()}");
            return;
        }
#endif

        // Section button active
        _buttonLeft.SetActive(!left);
        _buttonRight.SetActive(!right);

        // Section active
        _PlayMenus[0].SetActive(!(left ^ right));
        _PlayMenus[1].SetActive(left);
        _PlayMenus[2].SetActive(right);
        _MenuButtons[0].SetActive(left);
        _MenuButtons[1].SetActive(right);
    }


    private void OnLateMonthChange()
    {
        _dateText.text = $"{PlayManager.Instance.Year}{LanguageManager.Instance[TEX_YEAR]} {PlayManager.Instance.Month}{LanguageManager.Instance[TEX_MONTH]}";
    }


    private void OnLanguageChange()
    {
        OnLateMonthChange();
    }


    private IEnumerator NoiseBrightnessAnim()
    {
        float curNoiseBrt = _interactionBrightness;
        float brtGap = (_interactionBrightness - _normalBrightness) * 0.5f;
        float time = 0.0f;

        do
        {
            // Sets noise brightness.
            _noiseMaterial.SetColor(
                "_Colour",
                new Color(curNoiseBrt, curNoiseBrt, curNoiseBrt, _noiseAlpha)
            );

            // Time change
            time += Time.deltaTime;

            // Brightness change
            curNoiseBrt = Mathf.Cos(time * Mathf.PI / _noiseFadeOutTime) * brtGap + brtGap + _normalBrightness;
            yield return null;
        } while (time < _noiseFadeOutTime);

        // Sets noise brightness.
        _noiseMaterial.SetColor(
            "_Colour",
            new Color(_normalBrightness, _normalBrightness, _normalBrightness, _noiseAlpha)
        );

        // Coroutine end
        _noiseCoroutine = null;
    }


    private IEnumerator SectionMoveAnim(float camGoalPos)
    {
        Transform camTran = Camera.main.transform;
        float camIniPos = camTran.localPosition.x;
        float camlen = camGoalPos - camIniPos;
        float time = 0.0f;

        do
        {
            // Camera position change
            camTran.localPosition = new Vector3(
                //Mathf.Sin(time / _camMoveTime * 0.5f * Mathf.PI) * length + initialPos,
                (camIniPos - camGoalPos) / Mathf.Pow(-_camMoveTime, _camMovePow) * Mathf.Pow(time - _camMoveTime, _camMovePow) + camGoalPos,
                0.0f,
                0.0f
            );

            // Time change
            time += Time.deltaTime;

            yield return null;
        } while (time < _camMoveTime);

        // Sets camera position.
        camTran.localPosition = new Vector3(
            camGoalPos,
            0.0f,
            0.0f
        );

        // Coroutine end
        _cameraCoroutine = null;
    }


    private IEnumerator CostAnim(TextMeshProUGUI targetText, TextMeshProUGUI targetAmount)
    {
        float time = 0.0f;

        targetText.color = Color.white;
        targetText.fontStyle = FontStyles.Bold;
        targetAmount.color = Color.white;
        targetAmount.fontStyle = FontStyles.Bold;

        while (time < COST_ANIM_TIME)
        {
            time += Time.deltaTime;
            yield return null;
        }

        targetText.color = NORMALTEXT;
        targetText.fontStyle = FontStyles.Normal;
        targetAmount.color = NORMALTEXT;
        targetAmount.fontStyle = FontStyles.Normal;
    }


    private void Awake()
    {
        // Unity singletop pattern
        Instance = this;

        // Changes background material into copied material.
        _noiseMaterial = new Material(_noiseMaterial);
        foreach (Image image in _noiseBackgrounds)
        {
            image.material = _noiseMaterial;
        }

        // Game speed control colour
        _pauseButtonText.color = _deactiveColour;
        _speedButtonText.color = _activeColour;

        // Default noise brightness
        _noiseMaterial.SetColor(
            "_Colour",
            new Color(_normalBrightness, _normalBrightness, _normalBrightness, _noiseAlpha)
        );
    }


    private void Start()
    {
        FundUpdate();
        PlayManager.Instance.AddOnLateMonthChange(OnLateMonthChange);
        LanguageManager.Instance.AddOnLanguageChange(OnLanguageChange);
    }


    private void Update()
    {
        if (PlayManager.Instance.GamePause)
        {
            return;
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (_currentSection == 0)
            {
                GameMenu.Instance.ButtonGameMenuActive(true);
            }
            else
            {
                ButtonSection(-_currentSection);
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
        {
            if (_currentSection > -1)
            {
                ButtonSection(-1);
            }
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            if (_currentSection < 1)
            {
                ButtonSection(1);
            }
        }
    }
}
