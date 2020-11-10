using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Radar;
using UnityEditor.Events;

public class MissileWeaponWizard : EditorWindow
{

    private GameObject missileWeaponModel;

    private ModuleType moduleType;

    private Sprite menuSprite;

    private Missile missilePrefab;


    [MenuItem("Space Combat Kit/Create/Missile Weapon")]
    static void Init()
    {
        MissileWeaponWizard window = (MissileWeaponWizard)EditorWindow.GetWindow(typeof(MissileWeaponWizard), true, "Missile Weapon Wizard");
        window.Show();
    }

    private void Create()
    {

        // Create root gameobject

        GameObject gameObject = new GameObject("NewMissileWeapon");
        Selection.activeGameObject = gameObject;

        // Create 3D model

        if (missileWeaponModel != null)
        {
            GameObject meshObject = Instantiate(missileWeaponModel, gameObject.transform);
            meshObject.name = "Model";
            meshObject.transform.localPosition = Vector3.zero;
            meshObject.transform.localRotation = Quaternion.identity;
        }

        // Create spawn point

        GameObject spawnPointObject = new GameObject("MissileSpawnPoint");
        spawnPointObject.transform.parent = gameObject.transform;

        // Create and initialize necessary components

        Module module = gameObject.AddComponent<Module>();
        SerializedObject moduleSO = new SerializedObject(module);
        moduleSO.Update();

        MissileWeapon missileWeapon = gameObject.AddComponent<MissileWeapon>();
        SerializedObject missileWeaponSO = new SerializedObject(missileWeapon);
        missileWeaponSO.Update();

        TargetLocker targetLocker = gameObject.AddComponent<TargetLocker>();
        SerializedObject targetLockerSO = new SerializedObject(targetLocker);
        targetLockerSO.Update();

        Triggerable triggerable = gameObject.AddComponent<Triggerable>();
        SerializedObject triggerableSO = new SerializedObject(triggerable);
        triggerableSO.Update();

        Repeater repeater = gameObject.AddComponent<Repeater>();
        SerializedObject repeaterSO = new SerializedObject(repeater);
        repeaterSO.Update();

        ProjectileLauncher projectileLauncher = gameObject.AddComponent<ProjectileLauncher>();
        SerializedObject projectileLauncherSO = new SerializedObject(projectileLauncher);
        projectileLauncherSO.Update();

        // Link Unity Events

        UnityEventTools.AddPersistentListener(triggerable.onStartTriggering, repeater.StartTriggering);
        UnityEventTools.AddPersistentListener(triggerable.onStopTriggering, repeater.StopTriggering);
        UnityEventTools.AddPersistentListener(triggerable.onTriggerOnce, repeater.TriggerOnce);
        UnityEventTools.AddPersistentListener(repeater.OnAction, projectileLauncher.LaunchProjectile);

        // Update Module settings

        moduleSO.FindProperty("label").stringValue = "HOMING MISSILE";
        moduleSO.FindProperty("description").stringValue = "Homing missile.";

        if (moduleType != null)
        {
            moduleSO.FindProperty("moduleType").objectReferenceValue = moduleType;
        }
        if (menuSprite != null)
        {
            moduleSO.FindProperty("menuSprite").objectReferenceValue = menuSprite;
        }

        moduleSO.ApplyModifiedProperties();
        moduleSO.Update();

        // Update projectile launcher settings

        projectileLauncherSO.FindProperty("spawnPoint").objectReferenceValue = spawnPointObject;
        projectileLauncherSO.FindProperty("addLauncherVelocityToProjectile").boolValue = true;
        if (missilePrefab != null)
        {
            projectileLauncherSO.FindProperty("projectilePrefab").objectReferenceValue = missilePrefab.gameObject;
        }

        projectileLauncherSO.ApplyModifiedProperties();
        projectileLauncherSO.Update();

        // Update Missile Weapon settings

        missileWeaponSO.FindProperty("targetLocker").objectReferenceValue = targetLocker;
        missileWeaponSO.FindProperty("triggerable").objectReferenceValue = triggerable;
        missileWeaponSO.FindProperty("repeater").objectReferenceValue = repeater;
        missileWeaponSO.FindProperty("projectileLauncher").objectReferenceValue = projectileLauncher;

        missileWeaponSO.ApplyModifiedProperties();
        missileWeaponSO.Update();
      
    }

    protected void OnGUI()
    {

        missileWeaponModel = (GameObject)EditorGUILayout.ObjectField("3D Model", missileWeaponModel, typeof(GameObject), false);

        moduleType = (ModuleType)EditorGUILayout.ObjectField("Missile Module Type", moduleType, typeof(ModuleType), false);

        menuSprite = (Sprite)EditorGUILayout.ObjectField("Menu Sprite", menuSprite, typeof(Sprite), false);

        missilePrefab = (Missile)EditorGUILayout.ObjectField("Missile Prefab", missilePrefab, typeof(Missile), false);

        if (GUILayout.Button("Create"))
        {
            Create();
            Close();
        }
    }
}
