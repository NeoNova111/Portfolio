#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(TeleportScript)), CanEditMultipleObjects]
public class TeleportScriptEditor : Editor
{
    public SerializedProperty
        changeDestination_Prop,
        sceneName_Prop,
        destination_Prop,
        destinationHasNewMusic_Prop,
        newMusic_Prop,
        transitionType_Prop,
        transitionTime_Prop,
        fadeOutTime_Prop,
        transitionDisplay_Prop,
        transitionImage_Prop,
        dateText_Prop,
        depthText_Prop;

    private void OnEnable()
    {
        changeDestination_Prop = serializedObject.FindProperty("changeDestination");
        sceneName_Prop = serializedObject.FindProperty("sceneName");
        destination_Prop = serializedObject.FindProperty("destination");
        destinationHasNewMusic_Prop = serializedObject.FindProperty("destinationHasNewMusic");
        newMusic_Prop = serializedObject.FindProperty("newMusic");
        transitionType_Prop = serializedObject.FindProperty("transitionType");
        transitionTime_Prop = serializedObject.FindProperty("transitionTime");
        fadeOutTime_Prop = serializedObject.FindProperty("fadeOutTime");
        transitionDisplay_Prop = serializedObject.FindProperty("transitionDisplay");
        transitionImage_Prop = serializedObject.FindProperty("transitionImage");
        dateText_Prop = serializedObject.FindProperty("dateText");
        depthText_Prop = serializedObject.FindProperty("depthText");
    }

    public override void OnInspectorGUI()
    {
        var teleportScript = target as TeleportScript;

        serializedObject.Update();

        EditorGUILayout.PropertyField(changeDestination_Prop);
        DestinationChangeType dest = (DestinationChangeType)changeDestination_Prop.enumValueIndex;

        switch (dest)
        {
            case DestinationChangeType.CHANGESCENE:
                sceneName_Prop.stringValue = EditorGUILayout.TextField("Scene Name", sceneName_Prop.stringValue);
                break;

            case DestinationChangeType.TELEPORT:
                destination_Prop.vector3Value = EditorGUILayout.Vector3Field("Teleport Position", destination_Prop.vector3Value);
                break;
        }

        destinationHasNewMusic_Prop.boolValue = EditorGUILayout.Toggle("Has Different Music", destinationHasNewMusic_Prop.boolValue);
        if (destinationHasNewMusic_Prop.boolValue)
        {
            newMusic_Prop.objectReferenceValue = EditorGUILayout.ObjectField("New Music", newMusic_Prop.objectReferenceValue, typeof(AudioClip), true);
        }

        EditorGUILayout.PropertyField(transitionDisplay_Prop);
        TransitionDisplayType trans = (TransitionDisplayType)transitionDisplay_Prop.enumValueIndex;

        switch (trans)
        {
            case TransitionDisplayType.ADJUSTABLETEXT:
                dateText_Prop.stringValue = EditorGUILayout.TextField("Date Text", dateText_Prop.stringValue);
                depthText_Prop.stringValue = EditorGUILayout.TextField("Depth Text", depthText_Prop.stringValue);
                break;
            case TransitionDisplayType.IMAGE:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Transition Sprie");
                transitionImage_Prop.objectReferenceValue = EditorGUILayout.ObjectField(transitionImage_Prop.objectReferenceValue, typeof(Sprite), true);
                EditorGUILayout.EndHorizontal();
                break;
        }

        EditorGUILayout.PropertyField(transitionType_Prop);
        TransitionType type = (TransitionType)transitionType_Prop.enumValueIndex;

        switch (type)
        {
            case TransitionType.HARDCUT:
                transitionTime_Prop.floatValue = EditorGUILayout.FloatField("Transition Time", transitionTime_Prop.floatValue);
                break;
            case TransitionType.FADEOUT:
                transitionTime_Prop.floatValue = EditorGUILayout.FloatField("Transition Time", transitionTime_Prop.floatValue);
                fadeOutTime_Prop.floatValue = EditorGUILayout.FloatField("FadeOutTime Time", fadeOutTime_Prop.floatValue);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
