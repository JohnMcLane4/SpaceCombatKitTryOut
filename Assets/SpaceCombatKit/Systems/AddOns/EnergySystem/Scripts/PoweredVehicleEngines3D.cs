using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class PoweredVehicleEngines3D : VehicleEngines3D
    {

        [Header("Power")]

        [Tooltip("The Power component on this vehicle.")]
        [SerializeField]
        protected Power power;

        [Tooltip("The coefficients that are multiplied by the available 'direct' power to the engines to determine the rotation (steering) forces.")]
        [SerializeField]
        protected Vector3 powerToRotationForceCoefficients = new Vector3(0.1f, 0.1f, 0.2f);

        [Tooltip("The coefficients that are multiplied by the available 'direct' power to the engines to determine the translation (thrust) forces.")]
        [SerializeField]
        protected Vector3 powerToTranslationForceCoefficients = new Vector3(0.1f, 0.1f, 0.2f);


        protected override void Reset()
        {
            base.Reset();
            power = GetComponent<Power>();
        }

        private void Update()
        {

            if (power == null) return;

            // Calculate the current available pitch, yaw and roll torques
            if (power.GetPowerConfiguration(PoweredSubsystemType.Engines) != SubsystemPowerConfiguration.Unpowered)
            {
                availableSteeringForces = power.GetSubsystemTotalPower(PoweredSubsystemType.Engines) * powerToRotationForceCoefficients;
            }
            else
            {
                availableSteeringForces = defaultSteeringForces;
            }

            // Clamp below maximum limits
            availableSteeringForces.x = Mathf.Min(availableSteeringForces.x, maxSteeringForces.x);
            availableSteeringForces.y = Mathf.Min(availableSteeringForces.y, maxSteeringForces.y);
            availableSteeringForces.z = Mathf.Min(availableSteeringForces.z, maxSteeringForces.z);

            // Calculate the currently available thrust
            if (power.GetPowerConfiguration(PoweredSubsystemType.Engines) != SubsystemPowerConfiguration.Unpowered)
            {
                availableMovementForces = power.GetSubsystemTotalPower(PoweredSubsystemType.Engines) * powerToTranslationForceCoefficients;
            }
            else
            {
                availableMovementForces = defaultMovementForces;
            }

            // Keep the thrust below the maximum limit
            availableMovementForces.x = Mathf.Min(availableMovementForces.x, maxMovementForces.x);
            availableMovementForces.y = Mathf.Min(availableMovementForces.y, maxMovementForces.y);
            availableMovementForces.z = Mathf.Min(availableMovementForces.z, maxMovementForces.z);
            
        }
    }
}