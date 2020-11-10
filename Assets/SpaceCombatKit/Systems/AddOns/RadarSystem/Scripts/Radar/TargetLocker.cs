using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat.Radar
{
 
    /// <summary>
    /// Provides ability to lock onto a target with specified locking parameters
    /// </summary>
    public class TargetLocker : MonoBehaviour
    {

        [Header("Settings")]

        [SerializeField]
        protected Trackable target;
        public Trackable Target { get { return target; } }

        [SerializeField]
        protected float lockingTime = 3;
        public float LockingTime { get { return lockingTime; } }

        [SerializeField]
        protected float lockingAngle = 7;
        public float LockingAngle { get { return lockingAngle; } }

        [SerializeField]
        protected float lockingRange = 1000;
        public float LockingRange { get { return lockingRange; } }
        
        protected LockState lockState = LockState.NoLock;
        public LockState LockState { get { return lockState; } }

        protected float lockStateChangeTime = 0;
        public float LockStateChangeTime { get { return lockStateChangeTime; } }

        [SerializeField]
        protected bool lockingEnabled = true;

        [Header("Audio")]

        [SerializeField]
        protected bool audioEnabled = true;
        public bool AudioEnabled
        {
            get { return audioEnabled; }
            set 
            { 
                audioEnabled = value; 
                if (!audioEnabled)
                {
                    lockingAudio.Stop();
                    lockedAudio.Stop();                
                }
            }
        }

        [SerializeField]
        protected AudioSource lockingAudio;

        [SerializeField]
        protected AudioSource lockedAudio;

        [Header("Events")]

        // Target locking event
        public UnityEvent onLocking;

        // Target locked event
        public UnityEvent onLocked;

        // Target not locked event
        public UnityEvent onNoLock;



        /// <summary>
        /// Set the target for this target locker.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        public virtual void SetTarget(Trackable newTarget)
        {
            target = newTarget;
            SetLockState(LockState.NoLock);
        }


        /// <summary>
        /// Set the target for this target locker.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        /// <param name="lockState">The starting lock state for the new target.</param>
        public virtual void SetTarget(Trackable newTarget, LockState lockState)
        {
            target = newTarget;
            SetLockState(lockState);
        }


        /// <summary>
        /// Check if the target is in the lock zone.
        /// </summary>
        /// <returns>Whether target is in lock zone.</returns>
        public virtual bool TargetInLockZone()
        {

            // Check if target exists
            if (target == null) return false;

            // Check if target is in range
            if (Vector3.Distance(transform.position, target.transform.position) > lockingRange)
                return false;

            // Check if target is in locking angle
            if (Vector3.Angle(transform.forward, target.transform.position - transform.position) > lockingAngle)
                return false;

            return true;

        }


        /// <summary>
        /// Directly set the lock state.
        /// </summary>
        /// <param name="newState">The new lock state.</param>
        public virtual void SetLockState(LockState newState)
        {
            // Update lock state
            lockState = newState;
            lockStateChangeTime = Time.time;

            // Call the event
            switch (lockState)
            {
                case LockState.Locked:

                    if (audioEnabled)
                    {
                        if (lockingAudio != null)
                        {
                            lockingAudio.Stop();
                        }

                        if (lockedAudio != null)
                        {
                            lockedAudio.Play();
                        }
                    }

                    onLocked.Invoke();
                    break;

                case LockState.Locking:

                    if (audioEnabled && lockingAudio != null) lockingAudio.Play();
                    onLocking.Invoke();
                    break;

                case LockState.NoLock:

                    if (audioEnabled && lockingAudio != null) lockingAudio.Stop();
                    onNoLock.Invoke();
                    break;

            }       
        }


        // Called every frame
        public virtual void Update()
        {
            switch (lockState)
            {
                case LockState.NoLock:

                    if (TargetInLockZone())
                    {
                        SetLockState(LockState.Locking);
                    }

                    break;

                case LockState.Locking:

                    if (TargetInLockZone())
                    {
                        if (Time.time - lockStateChangeTime > lockingTime && lockingEnabled)
                        {
                            SetLockState(LockState.Locked);
                        }
                    }
                    else
                    {
                        SetLockState(LockState.NoLock);
                    }

                    break;

                case LockState.Locked:

                    if (!TargetInLockZone())
                    {
                        SetLockState(LockState.NoLock);
                    }

                    break;
            }
        }
    }
}
