using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TacticalAI.HealthScript))]
[CanEditMultipleObjects]
public class healthScriptInspector : Editor
{
    SerializedObject healthScript;

    SerializedProperty healthProp;
    SerializedProperty shieldsProp;
    SerializedProperty shieldsBlockDamageProp;
    SerializedProperty timeBeforeShieldRegenProp;
    SerializedProperty shieldRegenRateProp;

    SerializedProperty myTargetScriptProp;
    SerializedProperty myAIBaseScriptProp;
    SerializedProperty audioScriptProp;
    SerializedProperty rigidbodiesProp;
    SerializedProperty collidersToEnableProp;
    SerializedProperty rotateToAimGunScriptProp;
    SerializedProperty animatorProp;
    SerializedProperty gunScriptProp;
    SerializedProperty gunTransformProp;

    SerializedProperty staggerThreshhold;
    SerializedProperty canStagger;
    SerializedProperty staggerOdds;
    //SerializedProperty useDeathAnimation;

    void OnEnable()
    {
        healthScript = new SerializedObject(target);

        healthProp = serializedObject.FindProperty("health");
        shieldsProp = serializedObject.FindProperty("shields");
        shieldsBlockDamageProp = serializedObject.FindProperty("shieldsBlockDamage");
        timeBeforeShieldRegenProp = serializedObject.FindProperty("timeBeforeShieldRegen");
        shieldRegenRateProp = serializedObject.FindProperty("shieldRegenRate");

        myTargetScriptProp = serializedObject.FindProperty("myTargetScript");
        myAIBaseScriptProp = serializedObject.FindProperty("myAIBaseScript");
        rigidbodiesProp = serializedObject.FindProperty("rigidbodies");
        audioScriptProp = serializedObject.FindProperty("soundScript");
        collidersToEnableProp = serializedObject.FindProperty("collidersToEnable");
        rotateToAimGunScriptProp = serializedObject.FindProperty("rotateToAimGunScript");
        animatorProp = serializedObject.FindProperty("animator");
        gunScriptProp = serializedObject.FindProperty("gunScript");

        staggerThreshhold = serializedObject.FindProperty("staggerThreshhold");
        canStagger = serializedObject.FindProperty("canStagger");
        staggerOdds = serializedObject.FindProperty("staggerOdds");

        //useDeathAnimation = serializedObject.FindProperty("useDeathAnimation");
    }

    bool showLinks = false;

    public override void OnInspectorGUI()
    {
        healthScript.Update();
        EditorGUI.BeginChangeCheck();


        healthProp.floatValue = EditorGUILayout.FloatField("Health", healthProp.floatValue);
        shieldsProp.floatValue = EditorGUILayout.FloatField("Shields", shieldsProp.floatValue);
        shieldsBlockDamageProp.boolValue = EditorGUILayout.Toggle("Shields block damage", shieldsBlockDamageProp.boolValue);
        timeBeforeShieldRegenProp.floatValue = EditorGUILayout.FloatField("time before shield regeneration", timeBeforeShieldRegenProp.floatValue);
        shieldRegenRateProp.floatValue = EditorGUILayout.FloatField("Shield regeneration rate", shieldRegenRateProp.floatValue);

        timeBeforeShieldRegenProp.floatValue = EditorGUILayout.FloatField("time before shield regeneration", timeBeforeShieldRegenProp.floatValue);
        shieldRegenRateProp.floatValue = EditorGUILayout.FloatField("Shield regeneration rate", shieldRegenRateProp.floatValue);

        EditorGUILayout.PropertyField(staggerThreshhold);
        EditorGUILayout.PropertyField(canStagger);
        staggerOdds.floatValue = EditorGUILayout.Slider("Stagger Odds", staggerOdds.floatValue, 0.0f, 1.0f);

        //EditorGUILayout.PropertyField(useDeathAnimation);

        showLinks = EditorGUILayout.Foldout(showLinks, "Show linked components");
        if (showLinks)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(myAIBaseScriptProp);
            EditorGUILayout.PropertyField(myTargetScriptProp);
            DrawArray(rigidbodiesProp);
            DrawArray(collidersToEnableProp);
            EditorGUILayout.PropertyField(audioScriptProp);
            EditorGUILayout.PropertyField(rotateToAimGunScriptProp);
            EditorGUILayout.PropertyField(animatorProp);
            EditorGUILayout.PropertyField(gunScriptProp);

            //myAIBaseScriptProp.objectReferenceValue = EditorGUILayout.ObjectField("Base Script", myAIBaseScriptProp.objectReferenceValue, typeof(TacticalAI.BaseScript), true);
            //myTargetScriptProp.objectReferenceValue = EditorGUILayout.ObjectField("Target Script", myTargetScriptProp.objectReferenceValue, typeof(TacticalAI.TargetScript), true);
            //DrawArray(rigidbodiesProp);
            //audioScriptProp.objectReferenceValue = EditorGUILayout.ObjectField("Sound Script", audioScriptProp.objectReferenceValue, typeof(TacticalAI.SoundScript), true);
            //DrawArray(collidersToEnableProp);
            //rotateToAimGunScriptProp.objectReferenceValue = EditorGUILayout.ObjectField("Rotate To Aim Gun Script", rotateToAimGunScriptProp.objectReferenceValue, typeof(TacticalAI.RotateToAimGunScript), true);
            //animatorProp.objectReferenceValue = EditorGUILayout.ObjectField("Animator", animatorProp.objectReferenceValue, typeof(Animator), true);
            //gunScriptProp.objectReferenceValue = EditorGUILayout.ObjectField("Gun Script", gunScriptProp.objectReferenceValue, typeof(TacticalAI.GunScript), true);
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }

    public void DrawArray(SerializedProperty prop)
    {
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        EditorGUILayout.PropertyField(prop, true);
    }
}
