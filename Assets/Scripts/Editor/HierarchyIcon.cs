using UnityEngine;
using UnityEditor;

[InitializeOnLoadAttribute]
public class HierarchyIcon : Editor
{
    static HierarchyIcon()
    {
        EditorApplication.hierarchyWindowItemOnGUI += DrawIcon;
    }


    static void DrawIcon(int instanceID, Rect selectionRect)
    {
        // Get object
        GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceID);

        // Null check
        switch (go)
        {
            case null:
                return;
            default:
                break;
        }

        // Set rect
        Rect iconRect = new Rect(selectionRect.x, selectionRect.y + 1.0f, 15.0f, 15.0f);

        // Draw icon
        const float alpha = 0.3f; // Alpha
        if (go.name.StartsWith("Button"))
        {
            EditorGUI.DrawRect(
                iconRect,
                new Color(1.0f, 0.0f, 0.0f, alpha)
            );
        }
        else if (go.name.StartsWith("Text"))
        {
            EditorGUI.DrawRect(
                iconRect,
                new Color(0.0f, 1.0f, 0.0f, alpha)
            );
        }
        else if (go.name.StartsWith("Image"))
        {
            EditorGUI.DrawRect(
                iconRect,
                new Color(0.0f, 0.0f, 1.0f, alpha)
            );
        }
        else if (go.name.StartsWith("Background"))
        {
            EditorGUI.DrawRect(
                iconRect,
                new Color(0.0f, 0.0f, 0.0f, alpha)
            );
        }
        else if (go.name.StartsWith("Canvas"))
        {
            EditorGUI.DrawRect(
                iconRect,
                new Color(1.0f, 1.0f, 0.0f, alpha)
            );
        }
        else
        {
            EditorGUI.DrawRect(
                iconRect,
                new Color(1.0f, 1.0f, 1.0f, alpha)
            );
        }
    }
    
}
