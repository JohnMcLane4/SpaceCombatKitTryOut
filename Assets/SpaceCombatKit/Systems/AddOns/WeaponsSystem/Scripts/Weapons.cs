using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{
    [System.Serializable]
    public class OnTargetLockerLoadedEventHandler : UnityEvent<TargetLocker> { }

    [System.Serializable]
    public class OnTargetLockerUnloadedEventHandler : UnityEvent<TargetLocker> { }

    [System.Serializable]
    public class OnTargetLeaderLoadedEventHandler : UnityEvent<TargetLeader> { }

    [System.Serializable]
    public class OnTargetLeaderUnloadedEventHandler : UnityEvent<TargetLeader> { }


    public enum GimbaledWeaponsTargetingType
    {
        GimbalTargetTransform,
        SelectedTarget,
        NoTargeting
    }


    /// <summary>
    /// Manages the weapons loaded on a vehicle.
    /// </summary>
    public class Weapons : ModuleManager
    {
        [Tooltip("The target selector that the weapons use for lead target calculation and missile locking.")]
        [SerializeField]
        protected TargetSelector weaponsTargetSelector;
        public TargetSelector WeaponsTargetSelector { get { return weaponsTargetSelector; } }

        [SerializeField]
        protected bool aimAssistEnabled = false;

        [SerializeField]
        protected List<TargetLeader> targetLeaders = new List<TargetLeader>();
        public List<TargetLeader> TargetLeaders { get { return targetLeaders; } }

        protected List<TargetLocker> targetLockers = new List<TargetLocker>();
        public List<TargetLocker> TargetLockers { get { return targetLockers; } }

        protected List<AimAssist> aimAssists = new List<AimAssist>();
        public List<AimAssist> AimAssists { get { return aimAssists; } }

        protected List<GimbalController> gimbaledWeapons = new List<GimbalController>();
        public List<GimbalController> GimbaledWeapons { get { return gimbaledWeapons; } }

        [Header("Gimbaled Weapons")]

        [Tooltip("Targeting behavior for gimbaled weapons.")]
        [SerializeField]
        protected GimbaledWeaponsTargetingType gimbaledWeaponsTargetingType;

        [Tooltip("The target for gimbaled weapons.")]
        [SerializeField]
        protected Transform gimbalTargetTransform;

        [Tooltip("Whether gimbaled weapons should snap to the target (rather that smoothly rotating toward it).")]
        [SerializeField]
        protected bool snapToTarget;

        [Header("Events")]

        public OnTargetLockerLoadedEventHandler onTargetLockerLoaded;

        public OnTargetLockerUnloadedEventHandler onTargetLockerUnloaded;

        public OnTargetLeaderLoadedEventHandler onTargetLeaderLoaded;

        public OnTargetLeaderUnloadedEventHandler onTargetLeaderUnloaded;




        // Called when a module is mounted on one of the vehicle's module mounts.
        protected override void OnModuleMounted(Module module)
        { 
            // Look for target leading weapons
            TargetLeader targetLeader = module.GetComponent<TargetLeader>();
            if (targetLeader != null)
            {
                if (!targetLeaders.Contains(targetLeader))
                {
                    targetLeaders.Add(targetLeader);

                    onTargetLeaderLoaded.Invoke(targetLeader);
                }
            }

            // Look for locking weapons
            TargetLocker targetLocker = module.GetComponent<TargetLocker>();
            if (targetLocker != null)
            {
                if (!targetLockers.Contains(targetLocker))
                {
                    targetLockers.Add(targetLocker);

                    onTargetLockerLoaded.Invoke(targetLocker);
                }
            }

            // Look for aim assists
            AimAssist aimAssist = module.GetComponent<AimAssist>();
            if (aimAssist != null)
            {
                if (!aimAssists.Contains(aimAssist))
                {
                    aimAssist.IndependentUpdate = aimAssistEnabled;
                    aimAssists.Add(aimAssist);
                }
            }

            // Look for gimbaled weapons
            GimbalController gimbalController = module.GetComponentInChildren<GimbalController>();
            if (gimbalController != null)
            {
                if (!gimbaledWeapons.Contains(gimbalController))
                {
                    gimbaledWeapons.Add(gimbalController);
                }
            }
        }

        // Called when a module is unmounted from one of the vehicle's module mounts.
        protected override void OnModuleUnmounted(Module module)
        {
            // Unlink target leading weapons
            TargetLeader targetLeader = module.GetComponent<TargetLeader>();
            if (targetLeader != null)
            {
                if (targetLeaders.Contains(targetLeader))
                {
                    targetLeaders.Remove(targetLeader);
                }
            }

            // Unlink target locking weapons
            TargetLocker targetLocker = module.GetComponent<TargetLocker>();
            if (targetLocker != null)
            {
                if (targetLockers.Contains(targetLocker))
                {
                    targetLockers.Remove(targetLocker);
                }
            }

            // Unlink target locking weapons
            AimAssist aimAssist = module.GetComponent<AimAssist>();
            if (aimAssist != null)
            {
                if (aimAssists.Contains(aimAssist))
                {
                    aimAssists.Remove(aimAssist);
                }
            }

            // Look for gimbaled weapons
            GimbalController gimbalController = module.GetComponentInChildren<GimbalController>();
            if (gimbalController != null)
            {
                if (gimbaledWeapons.Contains(gimbalController))
                {
                    gimbaledWeapons.Remove(gimbalController);
                }
            }
        }

        public virtual void SetAimAssistEnabled(bool isEnabled)
        {
            for(int i = 0; i < aimAssists.Count; ++i)
            {
                aimAssists[i].IndependentUpdate = isEnabled;
            }

            aimAssistEnabled = isEnabled;
        }

        protected virtual void Update()
        {
            switch (gimbaledWeaponsTargetingType)
            {
                case GimbaledWeaponsTargetingType.GimbalTargetTransform:

                    for (int i = 0; i < gimbaledWeapons.Count; ++i)
                    {
                        float angleToTarget;
                        gimbaledWeapons[i].TrackPosition(gimbalTargetTransform.position, out angleToTarget, snapToTarget);
                    }
                    break;

                case GimbaledWeaponsTargetingType.SelectedTarget:

                    for (int i = 0; i < gimbaledWeapons.Count; ++i)
                    {
                        if (weaponsTargetSelector != null && weaponsTargetSelector.SelectedTarget != null)
                        {
                            float angleToTarget;
                            gimbaledWeapons[i].TrackPosition(weaponsTargetSelector.SelectedTarget.transform.position, out angleToTarget, snapToTarget);
                        }
                        
                    }
                    break;
            }
        }
    }
}