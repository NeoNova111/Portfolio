//#if UNITY_EDITOR
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using Cinemachine;
//using System;

//[CustomEditor(typeof(Room)), CanEditMultipleObjects]
//public class RoomCustomEditor : Editor
//{
//    public SerializedProperty
//        fears_Prop,
//        colliderArea_Prop,
//        topdownVC_Prop,
//        sideviewVC_Prop,
//        POV_VC_Prop,
//        cameraBlendTime_Prop,
//        perspective_Prop,
//        cameraOffset_Prop,
//        cameraDistance_Prop;

//    private void OnEnable()
//    {
//        fears_Prop = serializedObject.FindProperty("fears");
//        colliderArea_Prop = serializedObject.FindProperty("colliderArea");
//        topdownVC_Prop = serializedObject.FindProperty("topdownVC");
//        sideviewVC_Prop = serializedObject.FindProperty("sideviewVC");
//        POV_VC_Prop = serializedObject.FindProperty("POV_VC");
//        cameraBlendTime_Prop = serializedObject.FindProperty("cameraBlendTime");
//        perspective_Prop = serializedObject.FindProperty("perspective");
//        cameraOffset_Prop = serializedObject.FindProperty("cameraOffset");
//        cameraDistance_Prop = serializedObject.FindProperty("cameraDistance"); 
//    }

//    public override void OnInspectorGUI()
//    {
//        var teleportScript = target as Room;

//        serializedObject.Update();

//        colliderArea_Prop.objectReferenceValue = EditorGUILayout.ObjectField("Collider Box", colliderArea_Prop.objectReferenceValue, typeof(BoxCollider), true);

//        topdownVC_Prop.objectReferenceValue = EditorGUILayout.ObjectField("Topdown VC", topdownVC_Prop.objectReferenceValue, typeof(CinemachineVirtualCamera), true);
//        sideviewVC_Prop.objectReferenceValue = EditorGUILayout.ObjectField("SideView VC", sideviewVC_Prop.objectReferenceValue, typeof(CinemachineVirtualCamera), true);
//        POV_VC_Prop.objectReferenceValue = EditorGUILayout.ObjectField("POV VC", POV_VC_Prop.objectReferenceValue, typeof(CinemachineVirtualCamera), true);

//        cameraBlendTime_Prop.floatValue = EditorGUILayout.FloatField("Blendtime", cameraBlendTime_Prop.floatValue);

//        EditorGUILayout.PropertyField(perspective_Prop);
//        CameraPerspective pers = (CameraPerspective)perspective_Prop.enumValueIndex;

//        switch (pers)
//        {
//            case CameraPerspective.POV:
//                cameraOffset_Prop.vector3Value = EditorGUILayout.Vector3Field("Camera Offset", cameraOffset_Prop.vector3Value);
//                break;
//            case CameraPerspective.TOPDOWN:
//                cameraOffset_Prop.vector3Value = EditorGUILayout.Vector3Field("Camera Offset", cameraOffset_Prop.vector3Value);
//                break;
//            case CameraPerspective.SIDE:
//                cameraDistance_Prop.floatValue = EditorGUILayout.FloatField("Camera Distance", cameraDistance_Prop.floatValue);
//                break;
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}
//#endif
