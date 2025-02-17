using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenuManager : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [Header("Section Buttons")]
    [SerializeField] private GameObject _buttonLeft = null;
    [SerializeField] private GameObject _buttonRight = null;
    [Header("Play Menu")]
    [SerializeField] private GameObject[] _MenuButtons = new GameObject[2];
    [SerializeField] private GameObject[] _PlayMenus = new GameObject[3];
    [Header("Noise Material")]
    [SerializeField][Range(0.0f, 1.0f)] private float _normalBrightness = 0.3921568627450980392156862745098f;
    [SerializeField][Range(0.0f, 1.0f)] private float _interactionBrightness = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float _noiseAlpha = 0.94117647058823529411764705882353f;
    [SerializeField] private float _noiseFadeOutTime = 0.5f;
    [SerializeField] private Image[] _noiseBackgrounds = null;
    [SerializeField] private Material _noiseMaterial = null;
    private sbyte _currentSection = 0;


    public static PlayMenuManager Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonSection(bool isLeft)
    {
        if (isLeft)
        {
            // Left move
            SectionMove(-1);
        }
        else
        {
            // Right move
            SectionMove(1);
        }

        // Noise brightness change
        StopAllCoroutines();
        StartCoroutine(NoiseBrightnessAnim());
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

        // Noise brightness change
        StopAllCoroutines();
        StartCoroutine(NoiseBrightnessAnim());
    }



    /* ==================== Private Methods ==================== */

    private void SectionMove(sbyte direction)
    {
        // Section move
        _currentSection += direction;

        // Section active
        switch (_currentSection)
        {
            case -1:
                SectionActive(true, false);
                return;

            case 0:
                SectionActive(false, false);
                return;

            case 1:
                SectionActive(false, true);
                return;

            default:
#if UNITY_EDITOR
                Debug.LogError($"Wrong section number.\n{name}, PlayMenuManager, _currentSection : {_currentSection.ToString()}");
                _currentSection -= direction;
#endif
                return;
        }
    }


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

        // Default noise brightness
        _noiseMaterial.SetColor(
            "_Colour",
            new Color(_normalBrightness, _normalBrightness, _normalBrightness, _noiseAlpha)
        );
    }
}
