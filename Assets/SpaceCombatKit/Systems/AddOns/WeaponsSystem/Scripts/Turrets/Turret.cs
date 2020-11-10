using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// A turret is an gimballed weapon that automatically tracks and fires at a target.
    /// </summary>
    public class Turret : MonoBehaviour
    {
        [Header("Components")]

        [Tooltip("The turret gimbal controller.")]
        [SerializeField]
        protected GimbalController gimbalController;

        [Tooltip("The component for triggering the turret to fire.")]
        [SerializeField]
        protected Triggerable triggerable;

        [Tooltip("The component that performs target lead calculation for this turret.")]
        [SerializeField]
        protected TargetLeader targetLeader;

        [Header("Firing Parameters")]

        [Tooltip("The minimum time between firing bursts.")]
        [SerializeField]
        protected float minFiringInterval = 0.5f;

        [Tooltip("The maximum time between firing bursts.")]
        [SerializeField]
        protected float maxFiringInterval = 2;

        [Tooltip("The minimum length of the firing burst.")]
        [SerializeField]
        protected float minFiringPeriod = 1;

        [Tooltip("The maximum length of the firing burst.")]
        [SerializeField]
        protected float maxFiringPeriod = 2;

        [Tooltip("The maximum angle to target where the turret will fire.")]
        [SerializeField]
        protected float minFiringAngle = 5;

        [Header("General Settings")]

        [Tooltip("Whether the turret rotates back to the center when no target is present.")]
        [SerializeField]
        protected bool noTargetReturnToCenter = true;

        [Tooltip("Whether this turret controller is enabled.")]
        [SerializeField]
        protected bool controllerEnabled = true;
        public bool ControllerEnabled
        {
            get { return controllerEnabled; }
            set { controllerEnabled = value; }
        }

        // Firing state variables
        protected float firingAngle;
        protected float firingStateStartTime;
        protected bool firing = false;
        protected float nextFiringStatePeriod;


        // Update the firing of the turret
        protected void UpdateFiring()
        {
            bool canFire = true;

            // If angle to target is too large, can't fire
            if (firingAngle > minFiringAngle)
            {
                canFire = false;
            }

            if (canFire)
            {
                // Switch firing states
                if (Time.time - firingStateStartTime > nextFiringStatePeriod)
                {
                    if (firing)
                    {
                        StopFiring();
                    }
                    else
                    {
                        StartFiring();
                    }
                }
            }
            else
            {
                if (firing)
                {
                    StopFiring();
                }
            }
        }

        // Start firing the turret
        protected void StartFiring()
        {
            firing = true;
            nextFiringStatePeriod = Random.Range(minFiringPeriod, maxFiringPeriod);
            firingStateStartTime = Time.time;
            triggerable.StartTriggering();
        }

        // Stop firing the turret
        protected void StopFiring()
        {
            firing = false;
            nextFiringStatePeriod = Random.Range(minFiringInterval, maxFiringInterval);
            firingStateStartTime = Time.time;
            triggerable.StopTriggering();
        }

        // Called every frame
        protected void Update()
        {

            if (!controllerEnabled) return;

            // If a target exists ...
            if (targetLeader.Target != null)
            {
                // Track the target and fire
                gimbalController.TrackPosition(targetLeader.LeadTargetPosition, out firingAngle, false);
                UpdateFiring();
            }
            // If a target doesn't exist ...
            else
            {
                // Return the turret to center
                if (noTargetReturnToCenter) gimbalController.ResetGimbal(false);

                if (firing) StopFiring();
            }
        }
    }
}
