using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Base class for a missile weapon
    /// </summary>
    public class MissileWeapon : MonoBehaviour
    {

        [SerializeField]
        protected TargetLocker targetLocker;

        [SerializeField]
        protected Triggerable triggerable;

        [SerializeField]
        protected Repeater repeater;

        [SerializeField]
        protected ProjectileLauncher projectileLauncher;


        private void Awake()
        {
            if (projectileLauncher != null)
            {
                projectileLauncher.onProjectileLaunched.AddListener(OnMissileLaunched);
            }
        }

        /// <summary>
        /// Event called when a missile is launched.
        /// </summary>
        /// <param name="missileObject">The missile gameobject</param>
        public void OnMissileLaunched(GameObject missileObject)
        {
            Missile missile = missileObject.GetComponent<Missile>();
            if (missile == null)
            {
                Debug.LogWarning("Launched missile has no Missile component. Please add one.");
            }
            else
            {
                // Set missile parameters
                missile.SetTarget(targetLocker.Target);
                missile.SetLockState(targetLocker.LockState == LockState.Locked ? LockState.Locked : LockState.NoLock);
            }
        }


        public float GetMissileDamage(HealthType healthType)
        {
            HealthModifier healthModifier = projectileLauncher.ProjectilePrefab.GetComponentInChildren<HealthModifier>();
            if (healthModifier != null)
            {
                float damageValue = healthModifier.DefaultDamageValue;
                for (int i = 0; i < healthModifier.DamageOverrideValues.Count; ++i)
                {
                    if (healthModifier.DamageOverrideValues[i].HealthType == healthType)
                    {
                        damageValue = healthModifier.DamageOverrideValues[i].Value;
                    }
                }
                return damageValue;
            }
            else
            {
                return 0;
            }
        }


        public float GetMissileRange()
        {
            return projectileLauncher.ProjectilePrefab.GetComponent<TargetLocker>().LockingRange;
        }


        public float GetMissileSpeed()
        {
            return projectileLauncher.ProjectilePrefab.GetComponent<VehicleEngines3D>().GetDefaultMaxSpeedByAxis(false).magnitude;
        }


        public float GetMissileAgility()
        {
            return projectileLauncher.ProjectilePrefab.GetComponent<VehicleEngines3D>().MaxSteeringForces.magnitude;
        }
    }
}