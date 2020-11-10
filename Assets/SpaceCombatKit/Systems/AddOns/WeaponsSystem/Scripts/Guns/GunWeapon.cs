using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class GunWeapon : MonoBehaviour
    {
        
        public float GetDamage(HealthType healthType)
        {
            HealthModifier healthModifier = GetComponentInChildren<HealthModifier>();
            if (healthModifier == null)
            {
                ProjectileLauncher projectileLauncher = GetComponentInChildren<ProjectileLauncher>();
                if (projectileLauncher != null)
                {
                    healthModifier = projectileLauncher.ProjectilePrefab.GetComponent<HealthModifier>();
                }
            }

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

        public float GetSpeed()
        {
            ProjectileLauncher projectileLauncher = GetComponentInChildren<ProjectileLauncher>();
            if (projectileLauncher != null)
            {
                return projectileLauncher.ProjectilePrefab.GetComponent<RigidbodyMover>().LocalVelocity.magnitude;
            }

            BeamController beamController = GetComponentInChildren<BeamController>();
            if (beamController != null)
            {
                return float.MaxValue;
            }

            return 0;
        }
    }
}
