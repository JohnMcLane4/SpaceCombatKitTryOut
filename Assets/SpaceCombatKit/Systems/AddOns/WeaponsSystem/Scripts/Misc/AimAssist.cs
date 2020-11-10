using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{
    public class AimAssist : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField]
        protected bool independentUpdate = false;
        public bool IndependentUpdate
        {
            get { return independentUpdate; }
            set { independentUpdate = value; }
        }

        [SerializeField]
        protected Transform angleReferenceTransform;

        [SerializeField]
        protected Transform aimTransform;
        public Transform AimTransform { get { return aimTransform; } }

        [SerializeField]
        protected float aimAssistAngle;

        protected Transform currentTarget;

        [Header("Target")]

        [SerializeField]
        protected Transform target;

        [SerializeField]
        protected TargetLeader targetLeader;


        public void Aim(Vector3 targetPosition)
        {
            aimTransform.LookAt(targetPosition, aimTransform.parent.up);
        }

        public void ClearAim()
        {
            aimTransform.localRotation = Quaternion.identity;
        }

        protected virtual void UpdateAimAssist()
        {
            if (targetLeader != null)
            {
                if (targetLeader != null && targetLeader.Target != null)
                {
                    currentTarget = targetLeader.Target.transform;
                }
                else
                {
                    currentTarget = null;
                }
            }
            else
            {
                currentTarget = target;
            }

            if (currentTarget != null)
            {
                float angle = Vector3.Angle(angleReferenceTransform.forward, targetLeader.LeadTargetPosition - angleReferenceTransform.position);

                if (angle < aimAssistAngle)
                {
                    aimTransform.LookAt(targetLeader.LeadTargetPosition, angleReferenceTransform.up);
                }
                else
                {
                    aimTransform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                aimTransform.localRotation = Quaternion.identity;
            }
        }

        private void Update()
        {
            if (independentUpdate)
            {
                UpdateAimAssist();
            }
        }
    }
}