using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using Utility;

public class SnapperTool : EditorWindow
{
    public enum GridType
    {
        Cartesian,
        Polar
    }

    [MenuItem("DevTools/Snapper")]
    public static void Open() => GetWindow<SnapperTool>();

    public float gridSize = 1f;
    public float angularDivisions = 24;
    public GridType gridType = GridType.Cartesian;

    private SerializedObject so;
    private SerializedProperty _propGridSize;
    private SerializedProperty _propGridType;
    private SerializedProperty _propAngularDivisions;

    private void OnEnable()
    {
        so = new SerializedObject(this);
        _propGridSize = so.FindProperty("gridSize");
        _propGridType = so.FindProperty("gridType");
        _propAngularDivisions = so.FindProperty("angularDivisions");

        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI;

        LoadData();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;

        SaveData();
    }

    // For scene view.
    private void DuringSceneGUI(SceneView sceneView)
    {
        Handles.DrawLine(Vector3.zero, Vector3.up);
        Handles.zTest = CompareFunction.LessEqual;

        const float extend = 16f;
        Vector3 labelPos = new Vector3(extend * 2 / gridSize, 0, extend * 2 / gridSize);

        if (gridType == GridType.Cartesian)
        {
            DrawCartesianGrid(extend);
        }
        else
        {
            DrawGridPolar(extend);
        }

        Handles.Label(labelPos, "Grid for Snapping tool Grid Size: s " + gridSize);
    }

    // In order to make the normals of handle relative to your object you need to get your objects normal.
    private void DrawGridPolar(float extend)
    {
        int ringCount = Mathf.RoundToInt((extend) / gridSize);
        float radiusOuter = (ringCount - 1) * gridSize;
        // Radial grid (rings)
        for (int i = 1; i < ringCount; i++)
        {
            //default = vector3.zero
            Handles.DrawWireDisc(default, Vector3.up, i * gridSize);
        }

        // Angular grid (lines)
        for (int i = 0; i < angularDivisions; i++)
        {
            float t = i / (float)angularDivisions;

            // Angle, turns to radians
            float angRad = t * ExtensionMethods.TAU;
            float x = Mathf.Cos(angRad) * radiusOuter;
            float y = Mathf.Sin(angRad) * radiusOuter;

            //Vector2 dir = new Vector2(x,y); Use vector 3 to flatten the plane of grids
            Vector3 dir = new Vector3(x, 0, y);

            Handles.DrawAAPolyLine(Vector3.zero, dir);
        }
    }

    private void DrawCartesianGrid(float extend)
    {
        int lineCount = Mathf.RoundToInt((extend * 2) / gridSize);
        int halfLineCount = lineCount / 2;
        if (lineCount % 2 == 0)
            lineCount++;
        for (int i = 0; i < lineCount; i++)
        {
            float intOffset = i - halfLineCount;

            float xCoord = intOffset * gridSize;
            float zCoord0 = halfLineCount * gridSize;
            float zCoord1 = -halfLineCount * gridSize;
            Vector3 p0 = new Vector3(xCoord, 0f, zCoord0);
            Vector3 p1 = new Vector3(xCoord, 0f, zCoord1);
            Handles.DrawAAPolyLine(p0, p1);
            // Recalculate for vertical lines.
            p0 = new Vector3(zCoord0, 0f, xCoord);
            p1 = new Vector3(zCoord1, 0f, xCoord);
            Handles.DrawAAPolyLine(p0, p1);
        }
    }

    // For editor window.
    private void OnGUI()
    {
        // Before any controls so must be updated
        so.Update();
        EditorGUILayout.PropertyField(_propGridSize);
        EditorGUILayout.PropertyField(_propGridType);
        if (gridType == GridType.Polar)
        {
            EditorGUILayout.PropertyField(_propAngularDivisions);
            _propAngularDivisions.intValue = Mathf.Max(4, _propAngularDivisions.intValue);
        }
        so.ApplyModifiedProperties();

        // Similiar to horizontal grouping.
        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
        {
            if (GUILayout.Button("Snap Selection"))
            {
                SnapSelection();
            }
        }
    }

    private void SnapSelection()
    {
        foreach (var go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, "snap objects");
            go.transform.position = GetSnappedPosition(go.transform.position);
        }
    }

    Vector3 GetSnappedPosition(Vector3 originalPos)
    {
        if (gridType == GridType.Cartesian)
        {
            return originalPos.Round(gridSize);
        }
        if (gridType == GridType.Polar)
        {
            // Distance from center for current object.
            Vector2 vec = new Vector2(originalPos.x, originalPos.z);
            float dist = vec.magnitude;
            float snappedDist = dist.Round(gridSize);

            // Same for angle.
            float angRad = Mathf.Atan2(vec.y, vec.x);
            float angTurns = angRad / ExtensionMethods.TAU;
            // Convert from divisions (full turn) to the size of a single division angularly.
            float angSnapped = angTurns.Round(1F / angularDivisions);
            float angRadSnapped = angSnapped * ExtensionMethods.TAU;

            Vector2 snappedDir = new Vector2(Mathf.Cos(angRadSnapped), Mathf.Sin(angRadSnapped));
            Vector2 snappedVec = snappedDir * snappedDist;
            // Mapped the 2d space to vector 3.
            return new Vector3(snappedVec.x, originalPos.y, snappedVec.y);
        }

        return default;
    }

    private void LoadData()
    {
        // Load the data on window open.
        gridSize = EditorPrefs.GetFloat("SNAPPER_TOOL_gridSize", 1F);
        gridType = (GridType)EditorPrefs.GetFloat("SNAPPER_TOOL_gridType", 0);
        angularDivisions = EditorPrefs.GetInt("SNAPPER_TOOL_angularDivisions", 24);
    }

    private void SaveData()
    {
        // Save the data on window open.
        EditorPrefs.SetFloat("SNAPPER_TOOL_gridSize", gridSize);
        EditorPrefs.SetInt("SNAPPER_TOOL_gridType", (int)gridType);
        EditorPrefs.SetFloat("SNAPPER_TOOL_angularDivisions", angularDivisions);
    }
}
