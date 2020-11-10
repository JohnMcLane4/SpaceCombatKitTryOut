using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{
  
    public class GuidanceController : MonoBehaviour
    {
        [Header("Guidance")]

        [SerializeField]
        protected Vector3 targetPosition;

        [SerializeField]
        protected VehicleEngines3D engines;

        protected Vector3 controlValuesByAxis;

        [SerializeField]
        protected bool clearTargetPositionAfterUpdate = true;

        [Header("Maneuvring")]      

        [SerializeField]
        protected Vector3 maxRotationAngles = new Vector3(360, 360, 360);

        [Header("PID Controller")]

        [SerializeField]
        protected PIDController3D steeringPIDController;

        protected bool guidanceEnabled = false;


        public void SetGuidanceEnabled(bool guidanceEnabled)
        {
            this.guidanceEnabled = guidanceEnabled;
        }

        public virtual void SetTargetPosition(Vector3 newTargetPosition)
        {
            targetPosition = newTargetPosition;
        }

        // Update is called once per frame
        void Update()
        {
            if (guidanceEnabled)
            {
                Maneuvring.TurnToward(transform, targetPosition, maxRotationAngles, steeringPIDController);
                engines.SetSteeringInputs(steeringPIDController.GetControlValues());
            }
            else
            {
                engines.SetSteeringInputs(Vector3.zero);
            }
  
            engines.SetMovementInputs(new Vector3(0, 0, 1));
        }
    }
}