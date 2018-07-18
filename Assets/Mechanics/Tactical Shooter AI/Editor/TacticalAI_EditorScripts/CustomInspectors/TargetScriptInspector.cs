using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TacticalAI.TargetScript))]
[CanEditMultipleObjects]
public class TargetScriptInspector : Editor
{
    SerializedObject targetScript;

    SerializedProperty targetObjectTransformProp;
    SerializedProperty targetPriorityProp;
    SerializedProperty myLOSTargetProp;
    SerializedProperty myAIBaseScriptProp;
    SerializedProperty healthScriptHolderProp;

    //Target location stuff
    SerializedProperty myTeamIDProp;
    SerializedProperty alliedTeamsIDsProp;
    SerializedProperty enemyTeamsIDsProp;

    //Every x second, if we're not in cover, we'll check for LoS between all known targets.  If we can't find any, the agent will return to it's idle behavior
    SerializedProperty timeBeforeTargetExpirationProp;
    SerializedProperty timeBetweenTargetChecksIfEngagingProp;
    SerializedProperty timeBetweenTargetChecksIfNotEngagingProp;
    //SerializedProperty willEverLoseAwarenessProp;

    SerializedProperty timeBetweenLOSChecksProp;
    SerializedProperty maxLineOfSightChecksPerFrameProp;
    SerializedProperty eyeTransformProp;
    SerializedProperty myFieldOfViewProp;
    SerializedProperty debugFieldOfViewProp;

    //SerializedProperty shouldUseLOSTargets;
    //SerializedProperty currentEnemyTarget;

    SerializedProperty shoutDistProp;
    SerializedProperty timeBetweenReactingToSoundsProp;

    SerializedProperty canAcceptDynamicObjectRequestsProp;
    SerializedProperty distToLoseAwarenessProp;
    
    SerializedProperty maxDistToNoticeTarget;

    void OnEnable()
    {
        targetScript = new SerializedObject(target);

        targetObjectTransformProp = serializedObject.FindProperty("targetObjectTransform");
        targetPriorityProp = serializedObject.FindProperty("targetPriority");
        myLOSTargetProp = serializedObject.FindProperty("myLOSTarget");
        myAIBaseScriptProp = serializedObject.FindProperty("myAIBaseScript");
        healthScriptHolderProp = serializedObject.FindProperty("healthScriptHolder");

        myTeamIDProp = serializedObject.FindProperty("myTeamID");
        alliedTeamsIDsProp = serializedObject.FindProperty("alliedTeamsIDs");
        enemyTeamsIDsProp = serializedObject.FindProperty("enemyTeamsIDs");
        
        maxDistToNoticeTarget = serializedObject.FindProperty("maxDistToNoticeTarget");

        timeBeforeTargetExpirationProp = serializedObject.FindProperty("timeBeforeTargetExpiration");
        timeBetweenTargetChecksIfEngagingProp = serializedObject.FindProperty("timeBetweenTargetChecksIfEngaging");
        timeBetweenTargetChecksIfNotEngagingProp = serializedObject.FindProperty("timeBetweenTargetChecksIfNotEngaging");
        //willEverLoseAwarenessProp = serializedObject.FindProperty("willEverLoseAwareness");

        timeBetweenLOSChecksProp = serializedObject.FindProperty("timeBetweenLOSChecks");
        maxLineOfSightChecksPerFrameProp = serializedObject.FindProperty("maxLineOfSightChecksPerFrame");
        eyeTransformProp = serializedObject.FindProperty("eyeTransform");
        myFieldOfViewProp = serializedObject.FindProperty("myFieldOfView");
        debugFieldOfViewProp = serializedObject.FindProperty("debugFieldOfView");

        shoutDistProp = serializedObject.FindProperty("shoutDist");
        timeBetweenReactingToSoundsProp = serializedObject.FindProperty("timeBetweenReactingToSounds");

        canAcceptDynamicObjectRequestsProp = serializedObject.FindProperty("canAcceptDynamicObjectRequests");
        distToLoseAwarenessProp = serializedObject.FindProperty("distToLoseAwareness");
    }

    bool showLinks = false;
    bool teamIDStuff = false;
    bool targetCheckStuff = false;
    bool losCheckStuff = false;
    bool shoutDistance = false;

    public override void OnInspectorGUI()
    {
        targetScript.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(targetObjectTransformProp, true);
        EditorGUILayout.PropertyField(targetPriorityProp, true);
        EditorGUILayout.PropertyField(myLOSTargetProp, true);
        EditorGUILayout.PropertyField(canAcceptDynamicObjectRequestsProp, true);
        EditorGUILayout.Separator();

        teamIDStuff = EditorGUILayout.Foldout(teamIDStuff, "Show Team Parameters");
        if (teamIDStuff)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(myTeamIDProp, true);
            EditorGUILayout.PropertyField(alliedTeamsIDsProp, true);
            EditorGUILayout.PropertyField(enemyTeamsIDsProp, true);
            EditorGUI.indentLevel--;
        }

        targetCheckStuff = EditorGUILayout.Foldout(targetCheckStuff, "Show Target Check Parameters");
        if (targetCheckStuff)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(distToLoseAwarenessProp, true);
            EditorGUILayout.PropertyField(timeBeforeTargetExpirationProp, true);
            EditorGUILayout.PropertyField(timeBetweenTargetChecksIfEngagingProp, true);
            EditorGUILayout.PropertyField(timeBetweenTargetChecksIfNotEngagingProp, true);
            //EditorGUILayout.PropertyField(willEverLoseAwarenessProp, true);
            EditorGUI.indentLevel--;
        }

        losCheckStuff = EditorGUILayout.Foldout(losCheckStuff, "Show Line of Sight Parameters");
        if (losCheckStuff)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(timeBetweenLOSChecksProp, true);
            EditorGUILayout.PropertyField(maxLineOfSightChecksPerFrameProp, true);
            EditorGUILayout.PropertyField(eyeTransformProp, true);
            EditorGUILayout.PropertyField(myFieldOfViewProp, true);
            EditorGUILayout.PropertyField(debugFieldOfViewProp, true);
            EditorGUILayout.PropertyField(maxDistToNoticeTarget, true);                  
            EditorGUI.indentLevel--;
        }

        shoutDistance = EditorGUILayout.Foldout(shoutDistance, "Show Shout Paramaters");
        if (shoutDistance)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(shoutDistProp, true);
            EditorGUILayout.PropertyField(timeBetweenReactingToSoundsProp, true);
            EditorGUI.indentLevel--;
        }


        showLinks = EditorGUILayout.Foldout(showLinks, "Show linked components");
        if (showLinks)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(myAIBaseScriptProp, true);
            EditorGUILayout.PropertyField(healthScriptHolderProp, true);
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
