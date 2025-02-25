using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Constants;

public class MessageManager : MonoBehaviour
{
    /* ==================== Fields ==================== */

    [Header("Message Box")]
    [SerializeField] private GameObject _messageBox = null;
    [SerializeField] private TextMeshProUGUI _messageText = null;
    [Header("Message Box Position")]
    [SerializeField][Range(0.0f, 1.0f)] private float _messageBoxLength = 0.5f;
    [SerializeField] private float _mesMoveTime = 2.0f;
    [SerializeField] private float _mesMovePow = 2.0f;
    private Queue<string> _messageQueue = new Queue<string>();
    private RectTransform _mesRect = null;
    private Coroutine _messageTimeCount = null;
    private Coroutine _sectionMoveAnim = null;
    private float _time = MESSAGE_SHOW_TIME;

    static public MessageManager Instance { get; private set; }



    /* ==================== Public Methods ==================== */

    public void ButtonHide()
    {
        _messageBox.SetActive(false);
        AudioManager.Instance.Play(AudioType.Touch);
    }


    /// <summary>
    /// Enqueues a message.
    /// </summary>
    /// <param name="content">Message to show</param>
    public void EnqueueMessage(string content)
    {
        // Entire texts to show
        string entireText = $"{PlayManager.Instance.Year.ToString()}{LanguageManager.Instance["³â"]} {PlayManager.Instance.Month.ToString()}{LanguageManager.Instance["¿ù"]}\n{content}";

        if (_time >= MESSAGE_SHOW_TIME)
        {
            // Shows instantly.
            ShowMessage(entireText);
#if UNITY_EDITOR
            Debug.Log("Message showed instantly.");
#endif
        }
        else
        {
            // Enqueue
            _messageQueue.Enqueue(entireText);
#if UNITY_EDITOR
            Debug.Log("Message enqueued.");
#endif
        }

        // Message time count
        if (_messageTimeCount == null)
        {
            _messageTimeCount = StartCoroutine(CountTime());
        }
    }


    public void StartMesBoxMoveCoroutine(sbyte direction)
    {
        if (_sectionMoveAnim != null)
        {
            StopCoroutine(_sectionMoveAnim);
        }
        _sectionMoveAnim = StartCoroutine(SectionMoveAnim(direction));
    }



    /* ==================== Private Methods ==================== */

    private void ShowMessage(string text)
    {
        // Message show
        _messageText.text = text;
        _messageBox.SetActive(true);

        // Audio play
        AudioManager.Instance.Play(AudioType.Alert);

        // Initializes time.
        _time = 0.0f;
    }


    private IEnumerator CountTime()
    {
        while (true)
        {
            _time += Time.deltaTime;
            if (_time >= MESSAGE_SHOW_TIME)
            {
                if (_messageQueue.Count > 0)
                {
                    ShowMessage(_messageQueue.Peek());
#if UNITY_EDITOR
                    Debug.Log("Queued message showed.");
#endif
                }
                else
                {
                    _messageTimeCount = null;
                    break;
                }
            }
            yield return null;
        }
    }


    private IEnumerator SectionMoveAnim(sbyte direction)
    {
        float mesGoalPosMax;
        float mesGoalPosMin;
        float mesIniPosMax = _mesRect.anchorMax.x;
        float mesIniPosMin = _mesRect.anchorMin.x;
        float time = 0.0f;

        if (direction > 0)
        {
            mesGoalPosMax = 1.0f;
            mesGoalPosMin = 1.0f - _messageBoxLength;
        }
        else if (direction < 0)
        {
            mesGoalPosMin = 0.0f;
            mesGoalPosMax = _messageBoxLength;
        }
        else
        {
            mesGoalPosMax = 0.5f + _messageBoxLength * 0.5f;
            mesGoalPosMin = 0.5f - _messageBoxLength * 0.5f;
        }

        do
        {
            // Message box position change
            _mesRect.anchorMin = new Vector2(
                (mesIniPosMin - mesGoalPosMin) / Mathf.Pow(-_mesMoveTime, _mesMovePow) * Mathf.Pow(time - _mesMoveTime, _mesMovePow) + mesGoalPosMin,
                0.0f
            );
            _mesRect.anchorMax = new Vector2(
                (mesIniPosMax - mesGoalPosMax) / Mathf.Pow(-_mesMoveTime, _mesMovePow) * Mathf.Pow(time - _mesMoveTime, _mesMovePow) + mesGoalPosMax,
                1.0f
            );

            // Time change
            time += Time.deltaTime;

            yield return null;
        } while (time < _mesMoveTime);

        // Sets message box position.
        _mesRect.anchorMin = new Vector2(
            mesGoalPosMin,
            0.0f
        );
        _mesRect.anchorMax = new Vector2(
            mesGoalPosMax,
            1.0f
        );

        // Coroutine end
        _sectionMoveAnim = null;
    }


    private void Awake()
    {
        // Unity singleton patter
        Instance = this;

        _mesRect = _messageBox.GetComponent<RectTransform>();
    }
}
