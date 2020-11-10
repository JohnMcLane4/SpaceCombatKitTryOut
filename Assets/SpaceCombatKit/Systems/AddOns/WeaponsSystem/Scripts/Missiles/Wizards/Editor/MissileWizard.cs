using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Radar;
using UnityEditor.Events;

/*
public class MissileWizard : EditorWindow
{

    private GameObject missileModel;

    private bool targetProximityDetonation = true;

    private bool areaDamage = true;

    private GameObject exhaustVisualEffects;

    private AudioClip launchAudioClip;

    private AudioClip exhaustAudioClip;

    private GameObject explosion;

    
    [MenuItem("Space Combat Kit/Create/Missile")]
    static void Init()
    {
        MissileWizard window = (MissileWizard)EditorWindow.GetWindow(typeof(MissileWizard), true, "Missile Wizard");
        window.Show();
    }

    private void Create()
    {

        // Create root gameobject

        GameObject gameObject = new GameObject("NewMissile");
        Selection.activeGameObject = gameObject;

        // Create 3D model

        if (missileModel != null)
        {
            GameObject meshObject = Instantiate(missileModel, gameObject.transform);
            meshObject.name = "Model";
            meshObject.transform.localPosition = Vector3.zero;
            meshObject.transform.localRotation = Quaternion.identity;
        }

        // Create the visual effects

        if (exhaustVisualEffects != null)
        {
            GameObject exhaustVisualEffectsObject = Instantiate(exhaustVisualEffects, gameObject.transform);
            exhaustVisualEffectsObject.name = "ExhaustVisualEffects";
            exhaustVisualEffectsObject.transform.localPosition = Vector3.zero;
            exhaustVisualEffectsObject.transform.localRotation = Quaternion.identity;
        }

        // ************************ AUDIO ***************************

        // Create an object to store the audio under
        GameObject audioObject = new GameObject("Audio");
        audioObject.transform.parent = gameObject.transform;
        audioObject.transform.localPosition = Vector3.zero;
        audioObject.transform.localRotation = Quaternion.identity;

        // Create the launch audio

        if (launchAudioClip != null)
        {
            GameObject launchAudioObject = new GameObject("LaunchAudio");
            launchAudioObject.transform.parent = audioObject.transform;
            launchAudioObject.transform.localPosition = Vector3.zero;
            launchAudioObject.transform.localRotation = Quaternion.identity;

            AudioSource launchAudioSource = launchAudioObject.AddComponent<AudioSource>();
            launchAudioSource.clip = launchAudioClip;
            launchAudioSource.playOnAwake = true;
            launchAudioSource.loop = false;
        }

        // Create the exhaust audio

        if (exhaustAudioClip != null)
        {
            GameObject exhaustAudioObject = new GameObject("ExhaustAudio");
            exhaustAudioObject.transform.parent = audioObject.transform;
            exhaustAudioObject.transform.localPosition = Vector3.zero;
            exhaustAudioObject.transform.localRotation = Quaternion.identity;

            AudioSource exhaustAudioSource = exhaustAudioObject.AddComponent<AudioSource>();
            exhaustAudioSource.clip = exhaustAudioClip;
            exhaustAudioSource.playOnAwake = true;
            exhaustAudioSource.loop = true;
        }

        // ************************ Main Components ***************************

        // Add a rigidbody

        Rigidbody rBody = gameObject.AddComponent<Rigidbody>();
        rBody.useGravity = false;

        // Add the Missile component

        Missile missile = gameObject.AddComponent<Missile>();
        SerializedObject missileSO = new SerializedObject(missile);
        missileSO.Update();

        // Add a Target Locker

        TargetLocker targetLocker = gameObject.AddComponent<TargetLocker>();
        SerializedObject targetLockerSO = new SerializedObject(targetLocker);
        targetLockerSO.Update();

        targetLockerSO.FindProperty("lockingEnabled").boolValue = false;
        targetLockerSO.ApplyModifiedProperties();
        
        // Add a target leader

        TargetLeader targetLeader = gameObject.AddComponent<TargetLeader>();
        SerializedObject targetLeaderSO = new SerializedObject(targetLeader);
        targetLeaderSO.Update();

        // Add engines

        VehicleEngines3D engines = gameObject.AddComponent<VehicleEngines3D>();
        SerializedObject enginesSO = new SerializedObject(engines);
        enginesSO.Update();

        // Add a guidance system

        GuidanceController guidanceController = gameObject.AddComponent<GuidanceController>();
        SerializedObject guidanceControllerSO = new SerializedObject(guidanceController);
        guidanceControllerSO.Update();

        // Update the guidance system settings
        guidanceControllerSO.FindProperty("engines").objectReferenceValue = engines;
        guidanceControllerSO.ApplyModifiedProperties();

        // Add a Detonator

        Detonator detonator = gameObject.AddComponent<Detonator>();
        SerializedObject detonatorSO = new SerializedObject(detonator);
        detonatorSO.Update();

        if (explosion != null)
        {
            detonatorSO.FindProperty("detonatingDuration").floatValue = 2;
            detonatorSO.FindProperty("detonatingStateSpawnObjects").arraySize = 1;
            detonatorSO.FindProperty("detonatingStateSpawnObjects").GetArrayElementAtIndex(0).objectReferenceValue = explosion;
            detonatorSO.ApplyModifiedProperties();
        }

        UnityEventTools.AddBoolPersistentListener(detonator.onDetonating, engines.SetRigidbodyKinematic, true);
        UnityEventTools.AddBoolPersistentListener(detonator.onDetonated, gameObject.SetActive, false);
        UnityEventTools.AddBoolPersistentListener(detonator.onReset, engines.SetRigidbodyKinematic, false);

        // Add Health Modifier

        HealthModifier healthModifier = gameObject.AddComponent<HealthModifier>();
        SerializedObject healthModifierSO = new SerializedObject(healthModifier);
        healthModifierSO.Update();


        if (areaDamage)
        {
            // Add a damage receiver scanner for the area damage

            GameObject areaDamageScannerObject = new GameObject("AreaDamageScanner");
            areaDamageScannerObject.transform.parent = gameObject.transform;
            areaDamageScannerObject.transform.localPosition = Vector3.zero;
            areaDamageScannerObject.transform.localRotation = Quaternion.identity;

            // Add a kinematic rigidbody

            Rigidbody areaDamageScannerRigidbody = areaDamageScannerObject.AddComponent<Rigidbody>();
            areaDamageScannerRigidbody.isKinematic = true;

            // Add a sphere trigger collider and set the radius

            SphereCollider areaDamageScannerCollider = areaDamageScannerObject.AddComponent<SphereCollider>();
            areaDamageScannerCollider.isTrigger = true;
            areaDamageScannerCollider.radius = 20;

            // Add a damage receiver scanner

            DamageReceiverScanner areaDamageScanner = areaDamageScannerObject.AddComponent<DamageReceiverScanner>();
            SerializedObject areaDamageScannerSO = new SerializedObject(areaDamageScanner);
            areaDamageScannerSO.FindProperty("scannerTriggerCollider").objectReferenceValue = areaDamageScannerCollider;
            areaDamageScannerSO.ApplyModifiedProperties();

            healthModifierSO.FindProperty("areaDamageReceiverScanner").objectReferenceValue = areaDamageScanner;
            healthModifierSO.ApplyModifiedProperties();

        }
        
        // Add a collision scanner

        CollisionScanner collisionScanner = gameObject.AddComponent<CollisionScanner>();
        SerializedObject collisionScannerSO = new SerializedObject(collisionScanner);
        collisionScannerSO.Update();
            
        // Collision scanner settings
        if (areaDamage)
        {
            UnityEventTools.AddPersistentListener(collisionScanner.onHitDetected, healthModifier.RaycastHitAreaDamage);
        }
        else
        {
            UnityEventTools.AddPersistentListener(collisionScanner.onHitDetected, healthModifier.RaycastHitDamage);
        }

        UnityEventTools.AddPersistentListener(collisionScanner.onHitDetected, detonator.Detonate);
        

        if (targetProximityDetonation)
        {

            // Add a target proximity trigger to the root transform

            TargetProximityTrigger targetProximityTrigger = gameObject.AddComponent<TargetProximityTrigger>();
            SerializedObject targetProximityTriggerSO = new SerializedObject(targetProximityTrigger);
            targetProximityTriggerSO.Update();

            
            // Create an object for the proximity scanner trigger collider

            GameObject proximityTriggerColliderObject = new GameObject("TargetProximityScanner");
            proximityTriggerColliderObject.transform.parent = gameObject.transform;
            proximityTriggerColliderObject.transform.localPosition = Vector3.zero;
            proximityTriggerColliderObject.transform.localRotation = Quaternion.identity;

            // Add a kinematic rigidbody

            Rigidbody proximityTriggerColliderRigidbody = proximityTriggerColliderObject.AddComponent<Rigidbody>();
            proximityTriggerColliderRigidbody.isKinematic = true;

            // Add a sphere trigger collider and set the radius

            SphereCollider sphereCollider = proximityTriggerColliderObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 20;

            // Add a damage receiver scanner

            DamageReceiverScanner damageReceiverScanner = proximityTriggerColliderObject.AddComponent<DamageReceiverScanner>();
            SerializedObject damageReceiverScannerSO = new SerializedObject(damageReceiverScanner);
            damageReceiverScannerSO.Update();

            // Link the collider to the damage receiver scanner

            damageReceiverScannerSO.FindProperty("scannerTriggerCollider").objectReferenceValue = sphereCollider;
            damageReceiverScannerSO.ApplyModifiedProperties();
            damageReceiverScannerSO.Update();

            // Link the scanner to the proximity trigger

            targetProximityTriggerSO.FindProperty("scanner").objectReferenceValue = damageReceiverScanner;
            targetProximityTriggerSO.ApplyModifiedProperties();
            targetProximityTriggerSO.Update();

            UnityEventTools.AddPersistentListener(targetProximityTrigger.onTriggered, healthModifier.EmitDamage);
            UnityEventTools.AddPersistentListener(targetProximityTrigger.onTriggered, detonator.Detonate);

            UnityEventTools.AddPersistentListener(targetLocker.onLocked, targetProximityTrigger.SetTarget);

        }

        // Update the target locker settings

        UnityEventTools.AddPersistentListener(targetLocker.onLocked, targetLeader.SetTarget);
        UnityEventTools.AddVoidPersistentListener(targetLocker.onNoLock, targetLeader.ClearTarget);
        UnityEventTools.AddFloatPersistentListener(targetLocker.onNoLock, detonator.BeginDelayedDetonation, 4);
        
        // Update the target leader settings
        UnityEventTools.AddPersistentListener(targetLeader.onLeadTargetPositionUpdated, guidanceController.SetTargetPosition);

        missileSO.FindProperty("targetLocker").objectReferenceValue = targetLocker;
        missileSO.ApplyModifiedProperties();

    }

    protected void OnGUI()
    {
        EditorGUILayout.LabelField("Model", EditorStyles.boldLabel);

        missileModel = (GameObject)EditorGUILayout.ObjectField("3D Model", missileModel, typeof(GameObject), false);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

        targetProximityDetonation = EditorGUILayout.Toggle("Target Proximity Detonation", targetProximityDetonation);

        areaDamage = EditorGUILayout.Toggle("Area Damage", areaDamage);
        if (targetProximityDetonation == true)
        {
            areaDamage = true;
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Visual Effects", EditorStyles.boldLabel);

        exhaustVisualEffects = (GameObject)EditorGUILayout.ObjectField("Exhaust Visual Effects", exhaustVisualEffects, typeof(GameObject), false);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);

        launchAudioClip = (AudioClip)EditorGUILayout.ObjectField("Launch Audio Clip", launchAudioClip, typeof(AudioClip), false);

        exhaustAudioClip = (AudioClip)EditorGUILayout.ObjectField("Exhaust Audio Clip", exhaustAudioClip, typeof(AudioClip), false);

        EditorGUILayout.Space();

        explosion = (GameObject)EditorGUILayout.ObjectField("Explosion", explosion, typeof(GameObject), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create"))
        {
            Create();
            Close();
        }
    }
}
*/