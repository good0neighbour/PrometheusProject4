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
            return;
        }

        pmm.ButtonSection(true);
    }

    [MenuItem("PrometheusMission/ScenePlay Section Move/Right")]
    static public void MoveToRightSection()
    {
        PlayMenuManager pmm = FindFirstObjectByType<PlayMenuManager>(FindObjectsInactive.Exclude);
        if (pmm == null)
        {
            return;
        }

        pmm.ButtonSection(false);
    }
}
