#if UNITY_EDITOR
public enum LanguageType
{
    Korean,
    English,
    Russian,
    Japanese,
    French,
    End
}
#endif


public enum AudioType
{
    Touch,
    Select,
    Unable,
    Alert,
    End
}

public enum GamePauseType
{
    Pause = 1,
    Resume = -1
}