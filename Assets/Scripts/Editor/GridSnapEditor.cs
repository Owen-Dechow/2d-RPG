using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSnap))]
public class GridSnapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        if (GUILayout.Button("Snap Transform To Grid"))
        {
            GridSnap door = (GridSnap)target;
            door.SnapTransformToGrid();
        }
    }
}