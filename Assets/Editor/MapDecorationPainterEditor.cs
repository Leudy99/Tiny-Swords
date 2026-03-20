#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapDecorationPainter))]
public class MapDecorationPainterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        MapDecorationPainter painter = (MapDecorationPainter)target;

        if (GUILayout.Button("Generate Decorations"))
        {
            painter.GenerateDecorations();
        }

        if (GUILayout.Button("Clear Decorations"))
        {
            painter.ClearDecorations();
        }
    }
}
#endif