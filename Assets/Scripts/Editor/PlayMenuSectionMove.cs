using UnityEngine;
using UnityEditor;

public class PlayMenuSectionMove : Editor
{
    [MenuItem("PrometheusMission/ScenePlay Section Move/Left")]
    static public void MoveToLeftSection()
    {
        PlayMenuManager pmm = FindFirstObjectByType<PlayMenuManager>(FindObjectsInactive.Exclude);
        if (pmm == null)
        {
            Debug.LogWarning("Only functioning on ScenePlay.\nPlayMenuManager, MoveToLeftSection()");
            return;
        }

        pmm.ButtonSection(-1);
    }

    [MenuItem("PrometheusMission/ScenePlay Section Move/Right")]
    static public void MoveToRightSection()
    {
        PlayMenuManager pmm = FindFirstObjectByType<PlayMenuManager>(FindObjectsInactive.Exclude);
        if (pmm == null)
        {
            Debug.LogWarning("Only functioning on ScenePlay\nPlayMenuManager, MoveToRightSection()");
            return;
        }

        pmm.ButtonSection(1);
    }
}
