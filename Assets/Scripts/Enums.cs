#if UNITY_EDITOR
public enum LanguageType : byte
{
    Korean,
    English,
    Russian,
    Japanese,
    French,
    End
}
#endif


public enum AudioType : byte
{
    Touch,
    Select,
    Unable,
    Alert,
    Failed,
    End
}


public enum BottomInfoType : byte
{
    Fund,
    Research,
    Culture,
    Electricity,
    Stone,
    Iron,
    Heavy,
    Precious,
    Nuclear,
    End
}


public enum TechTreeType : byte
{
    Facilities,
    Technologies,
    Thoughts,
    End
}


public enum TechStatus : byte
{
    Unavailable,
    Available,
    Adopted
}