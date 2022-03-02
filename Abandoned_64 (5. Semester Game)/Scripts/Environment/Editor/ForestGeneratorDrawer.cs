using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ForestGenerator))]
public class ForestGeneratorDrawer : Editor
{
    public SerializedProperty treePrefab_Prop, sprites_Prop, rowSpacing_Prop, rows_Prop;

    private void OnEnable()
    {
        treePrefab_Prop = serializedObject.FindProperty("billboardPrefab");
        sprites_Prop = serializedObject.FindProperty("treeMaterials");
        rowSpacing_Prop = serializedObject.FindProperty("spaceBetweenRows");
        rows_Prop = serializedObject.FindProperty("rows");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ForestGenerator gen = (ForestGenerator)target;

        EditorGUILayout.PropertyField(treePrefab_Prop, new GUIContent("Tree Billboard Prefab"));
        EditorGUILayout.PropertyField(sprites_Prop, new GUIContent("Tree Materials"));
        EditorGUILayout.PropertyField(rowSpacing_Prop, new GUIContent("Row Spacing"));
        EditorGUILayout.PropertyField(rows_Prop, new GUIContent("Rows"));

        if (GUILayout.Button("Generate"))
        {
            gen.GenerateForest();
        }

        serializedObject.ApplyModifiedProperties();
    }

}
