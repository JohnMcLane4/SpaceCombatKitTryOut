using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Base class for a guided missile.
    /// </summary>
    public class Missile : Projectile
    {

        [SerializeField]
        protected TargetLocker targetLocker;

        [SerializeField]
        protected TargetLeader targetLeader;

        [SerializeField]
        protected GuidanceController guidanceController;

        [SerializeField]
        protected Detonator detonator;

        [SerializeField]
        protected float noLockLifetime = 4;

        protected Rigidbody m_Rigidbody;

        protected bool locked = false;


        protected override void Awake()
        {
            base.Awake();
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Set the target.
        /// </summary>
        /// <param name="target">The new target.</param>
        public virtual void SetTarget(Trackable target)
        {
            if (targetLocker != null) targetLocker.SetTarget(target);
            if (targetLeader != null) targetLeader.SetTarget(target);
            if (guidanceController != null) guidanceController.SetGuidanceEnabled(true);
        }

        /// <summary>
        /// Set the lock state of the missile.
        /// </summary>
        /// <param name="lockState">The new lock state.</param>
        public virtual void SetLockState(LockState lockState)
        {
            if (targetLocker != null) targetLocker.SetLockState(lockState);

            locked = true;
        }

        private void Update()
        {
            if (targetLocker.LockState == LockState.Locked)
            {
                targetLeader.InterceptSpeed = m_Rigidbody.velocity.magnitude;
                guidanceController.SetTargetPosition(targetLeader.LeadTargetPosition);
            }
            else
            {
                if (locked)
                {
                    guidanceController.SetGuidanceEnabled(false);
                    detonator.BeginDelayedDetonation(noLockLifetime);
                }
                
            }
        }
    }
}