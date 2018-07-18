using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TacticalAI.GunScript))]
[CanEditMultipleObjects]
public class GunScriptInspector : Editor
{
    SerializedObject gunScript;


    SerializedProperty myAIBaseScriptProp;
    SerializedProperty animationScriptProp;
    SerializedProperty audioSourceProp;
    //SerializedProperty gunTransformProp;

    SerializedProperty bulletObjectProp;
    SerializedProperty bulletSoundProp;
    SerializedProperty pelletsPerShotProp;
    SerializedProperty isRocketLauncherProp;
    SerializedProperty bulletSoundVolumeProp;
    SerializedProperty bulletSpawnProp;

    SerializedProperty muzzleFlashProp;
    SerializedProperty muzzleFlashSpawnProp;
    SerializedProperty flashDestroyTimeProp;

    //Secondary Fire
    SerializedProperty secondaryFireObjectProp;
    SerializedProperty oddsToSecondaryFireProp;
    SerializedProperty minDistForSecondaryFireProp;
    SerializedProperty maxDistForSecondaryFireProp;
    SerializedProperty needsLOSForSecondaryFireProp;
    SerializedProperty minTimeBetweenSecondaryFireProp;	

    //Rate of Fire
    SerializedProperty minPauseTimeProp;
    SerializedProperty randomPauseTimeAddProp;
    SerializedProperty minRoundsPerVolleyProp;
    SerializedProperty maxRoundsPerVolleyProp;
    SerializedProperty rateOfFireProp;
    SerializedProperty burstFireRateProp;
    SerializedProperty shotsPerBurstProp;

    SerializedProperty bulletsUntilReloadProp;
    SerializedProperty reloadSoundProp;
    SerializedProperty reloadSoundVolumeProp;
    SerializedProperty reloadTimeProp;

    SerializedProperty inaccuracyProp;
    SerializedProperty maxFiringAngleProp;
    SerializedProperty maxSecondaryFireAngleProp;

    SerializedProperty timeBetweenLOSChecksProp;
    SerializedProperty distInFrontOfTargetAllowedForCoverProp;
    SerializedProperty coverTransitionTimeProp;
    SerializedProperty soundRadiusProp;

    SerializedProperty minDistToFireGunProp;
    SerializedProperty maxDistToFireGunProp;

    SerializedProperty grenadeSpawn;

    void OnEnable()
    {
        gunScript = new SerializedObject(target);

        myAIBaseScriptProp = serializedObject.FindProperty("myAIBaseScript");
        animationScriptProp = serializedObject.FindProperty("animationScript");
        audioSourceProp = serializedObject.FindProperty("audioSource");
        //gunTransformProp = serializedObject.FindProperty("gunTransform");

		minDistToFireGunProp = serializedObject.FindProperty("minimumDistToFireGun");
        maxDistToFireGunProp = serializedObject.FindProperty("maximumDistToFireGun");


        bulletObjectProp = serializedObject.FindProperty("bulletObject");
        bulletSpawnProp = serializedObject.FindProperty("bulletSpawn");
        pelletsPerShotProp = serializedObject.FindProperty("pelletsPerShot");
        isRocketLauncherProp = serializedObject.FindProperty("isRocketLauncher");
        bulletSoundProp = serializedObject.FindProperty("bulletSound");
        bulletSoundVolumeProp = serializedObject.FindProperty("bulletSoundVolume");

        muzzleFlashProp = serializedObject.FindProperty("muzzleFlash");
        muzzleFlashSpawnProp = serializedObject.FindProperty("muzzleFlashSpawn");
        flashDestroyTimeProp = serializedObject.FindProperty("flashDestroyTime");

        secondaryFireObjectProp = serializedObject.FindProperty("secondaryFireObject");
        oddsToSecondaryFireProp = serializedObject.FindProperty("oddsToSecondaryFire");
        minDistForSecondaryFireProp = serializedObject.FindProperty("minDistForSecondaryFire");
        maxDistForSecondaryFireProp = serializedObject.FindProperty("maxDistForSecondaryFire");
        needsLOSForSecondaryFireProp = serializedObject.FindProperty("needsLOSForSecondaryFire");
        minTimeBetweenSecondaryFireProp = serializedObject.FindProperty("minTimeBetweenSecondaryFire");

        minPauseTimeProp = serializedObject.FindProperty("minPauseTime");
        randomPauseTimeAddProp = serializedObject.FindProperty("randomPauseTimeAdd");
        minRoundsPerVolleyProp = serializedObject.FindProperty("minRoundsPerVolley");
        maxRoundsPerVolleyProp = serializedObject.FindProperty("maxRoundsPerVolley");
        rateOfFireProp = serializedObject.FindProperty("rateOfFire");
        burstFireRateProp = serializedObject.FindProperty("burstFireRate");
        shotsPerBurstProp = serializedObject.FindProperty("shotsPerBurst");

        bulletsUntilReloadProp = serializedObject.FindProperty("bulletsUntilReload");
        reloadSoundProp = serializedObject.FindProperty("reloadSound");
        reloadSoundVolumeProp = serializedObject.FindProperty("reloadSoundVolume");
        reloadTimeProp = serializedObject.FindProperty("reloadTime");

        inaccuracyProp = serializedObject.FindProperty("inaccuracy");
        maxFiringAngleProp = serializedObject.FindProperty("maxFiringAngle");
        maxSecondaryFireAngleProp = serializedObject.FindProperty("maxSecondaryFireAngle");

        timeBetweenLOSChecksProp = serializedObject.FindProperty("timeBetweenLOSChecks");
        distInFrontOfTargetAllowedForCoverProp = serializedObject.FindProperty("distInFrontOfTargetAllowedForCover");
        coverTransitionTimeProp = serializedObject.FindProperty("coverTransitionTime");
        soundRadiusProp = serializedObject.FindProperty("soundRadius");

        grenadeSpawn = serializedObject.FindProperty("grenadeSpawn");

    }

    bool showScripts = false;
    bool showFlashStuff = false;
    bool showSecondaryFireStuff = false;
    bool showRateOfFireStuff = false;
    bool showClipStuff = false;
    bool showAccuracyStuff = false;
    bool showMiscStuff = false;

    public override void OnInspectorGUI()
    {
        gunScript.Update();
        EditorGUI.BeginChangeCheck();

        //bulletObjectProp.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Object", bulletObjectProp.objectReferenceValue, typeof(GameObject), true);
        //bulletSpawnProp.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Spawn", bulletSpawnProp.objectReferenceValue, typeof(Transform), true);
        //bulletSoundProp.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Sound", bulletSoundProp.objectReferenceValue, typeof(AudioClip), true);
        EditorGUILayout.PropertyField(bulletObjectProp, true);
        EditorGUILayout.PropertyField(bulletSpawnProp, true);
        EditorGUILayout.PropertyField(bulletSoundProp, true);
        bulletSoundVolumeProp.floatValue = EditorGUILayout.Slider("Bullet Sound Volume", bulletSoundVolumeProp.floatValue, 0.0f, 1.0f);
        soundRadiusProp.floatValue = EditorGUILayout.FloatField("Sound Radius", soundRadiusProp.floatValue);
        isRocketLauncherProp.boolValue = EditorGUILayout.Toggle("Is Rocket Launcher", isRocketLauncherProp.boolValue);
        EditorGUILayout.Separator();

        showFlashStuff = EditorGUILayout.Foldout(showFlashStuff, "Show Muzzle Flash Parameters");
        if (showFlashStuff)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(muzzleFlashProp, true);
            EditorGUILayout.PropertyField(muzzleFlashSpawnProp, true);
            //muzzleFlashProp.objectReferenceValue = EditorGUILayout.ObjectField("Flash Object", muzzleFlashProp.objectReferenceValue, typeof(GameObject), true);
            //muzzleFlashSpawnProp.objectReferenceValue = EditorGUILayout.ObjectField("Flash Spawn", muzzleFlashSpawnProp.objectReferenceValue, typeof(Transform), true);
            flashDestroyTimeProp.floatValue = EditorGUILayout.FloatField("Flash Duration", flashDestroyTimeProp.floatValue);
            EditorGUI.indentLevel--;
        }
        //EditorGUILayout.Separator();

        showSecondaryFireStuff = EditorGUILayout.Foldout(showSecondaryFireStuff, "Show Secondary Fire Parameters");
        if (showSecondaryFireStuff)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(secondaryFireObjectProp, true);
            //secondaryFireObjectProp.objectReferenceValue = EditorGUILayout.ObjectField("Secondary fire object", secondaryFireObjectProp.objectReferenceValue, typeof(GameObject), true);
            EditorGUILayout.PropertyField(grenadeSpawn, true);          
            oddsToSecondaryFireProp.floatValue = EditorGUILayout.Slider("Secondary fire odds", oddsToSecondaryFireProp.floatValue, 0.0f, 1.0f);
            minDistForSecondaryFireProp.floatValue = EditorGUILayout.FloatField("Min dist for secondary fire", minDistForSecondaryFireProp.floatValue);
            maxDistForSecondaryFireProp.floatValue = EditorGUILayout.FloatField("Max dist for secondary fire", maxDistForSecondaryFireProp.floatValue);
            needsLOSForSecondaryFireProp.boolValue = EditorGUILayout.Toggle("Need Line of sight for secondary fire", needsLOSForSecondaryFireProp.boolValue);
            minTimeBetweenSecondaryFireProp.floatValue = EditorGUILayout.FloatField("Min time between secondary fire", minTimeBetweenSecondaryFireProp.floatValue);
            EditorGUI.indentLevel--;
        }
        //EditorGUILayout.Separator();

        showRateOfFireStuff = EditorGUILayout.Foldout(showRateOfFireStuff, "Show Rate of Fire Parameters");
        if (showRateOfFireStuff)
        {
            EditorGUI.indentLevel++;
            minPauseTimeProp.floatValue = EditorGUILayout.FloatField("Min time between volleys", minPauseTimeProp.floatValue);
            randomPauseTimeAddProp.floatValue = EditorGUILayout.FloatField("Max extra time between volleys", randomPauseTimeAddProp.floatValue);
            minRoundsPerVolleyProp.intValue = EditorGUILayout.IntField("Min rounds per volley", minRoundsPerVolleyProp.intValue);
            maxRoundsPerVolleyProp.intValue = EditorGUILayout.IntField("Max rounds per volley", maxRoundsPerVolleyProp.intValue);
            rateOfFireProp.floatValue = EditorGUILayout.FloatField("Rate of fire", rateOfFireProp.floatValue);
            burstFireRateProp.floatValue = EditorGUILayout.FloatField("Burst rate of fire", burstFireRateProp.floatValue);
            shotsPerBurstProp.intValue = EditorGUILayout.IntField("Shots per burst", shotsPerBurstProp.intValue);
            pelletsPerShotProp.intValue = EditorGUILayout.IntField("Pellets Per Shot", pelletsPerShotProp.intValue);
            EditorGUI.indentLevel--;
        }
        //EditorGUILayout.Separator();

        showClipStuff = EditorGUILayout.Foldout(showClipStuff, "Show Clip Parameters");
        if (showClipStuff)
        {
            EditorGUI.indentLevel++;
            bulletsUntilReloadProp.intValue = EditorGUILayout.IntField("Clip size", bulletsUntilReloadProp.intValue);
            EditorGUILayout.PropertyField(reloadSoundProp, true);
            //reloadSoundProp.objectReferenceValue = EditorGUILayout.ObjectField("Reload Sound", reloadSoundProp.objectReferenceValue, typeof(AudioClip), true);
            reloadSoundVolumeProp.floatValue = EditorGUILayout.Slider("Reload Sound Volume", reloadSoundVolumeProp.floatValue, 0.0f, 1.0f);
            reloadTimeProp.floatValue = EditorGUILayout.FloatField("Reload Time", reloadTimeProp.floatValue);
            EditorGUI.indentLevel--;
        }
        //EditorGUILayout.Separator();

        showAccuracyStuff = EditorGUILayout.Foldout(showAccuracyStuff, "Show Accuracy Parameters");
        if (showAccuracyStuff)
        {
            EditorGUI.indentLevel++;
            inaccuracyProp.floatValue = EditorGUILayout.FloatField("Inaccruracy", inaccuracyProp.floatValue);
            maxFiringAngleProp.floatValue = EditorGUILayout.FloatField("Max firing angle", maxFiringAngleProp.floatValue);
            maxSecondaryFireAngleProp.floatValue = EditorGUILayout.FloatField("Max secondary firing angle", maxSecondaryFireAngleProp.floatValue);
            EditorGUI.indentLevel--;
        }
        //EditorGUILayout.Separator();

        showMiscStuff = EditorGUILayout.Foldout(showMiscStuff, "Show Miscellaneous Parameters");
        if (showMiscStuff)
        {
            EditorGUI.indentLevel++;
            timeBetweenLOSChecksProp.floatValue = EditorGUILayout.FloatField("Time between line of sight checks", timeBetweenLOSChecksProp.floatValue);
            distInFrontOfTargetAllowedForCoverProp.floatValue = EditorGUILayout.FloatField("Dist allowed in Front of target for cover", distInFrontOfTargetAllowedForCoverProp.floatValue);
            coverTransitionTimeProp.floatValue = EditorGUILayout.FloatField("Cover Transition Time", coverTransitionTimeProp.floatValue);
            EditorGUILayout.PropertyField(minDistToFireGunProp, true);
            EditorGUILayout.PropertyField(maxDistToFireGunProp, true);
            EditorGUI.indentLevel--;
        }
        //EditorGUILayout.Separator();

        showScripts = EditorGUILayout.Foldout(showScripts, "Show linked components");
        if (showScripts)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(myAIBaseScriptProp, true);
            EditorGUILayout.PropertyField(animationScriptProp, true);
            EditorGUILayout.PropertyField(audioSourceProp, true);
            //EditorGUILayout.PropertyField(gunTransformProp, true);

            
            //myAIBaseScriptProp.objectReferenceValue = EditorGUILayout.ObjectField("Base Script", myAIBaseScriptProp.objectReferenceValue, typeof(TacticalAI.BaseScript), true);
            //animationScriptProp.objectReferenceValue = EditorGUILayout.ObjectField("Animation Script", animationScriptProp.objectReferenceValue, typeof(TacticalAI.AnimationScript), true);
            //audioSourceProp.objectReferenceValue = EditorGUILayout.ObjectField("Audio Source", audioSourceProp.objectReferenceValue, typeof(AudioSource), true);
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
