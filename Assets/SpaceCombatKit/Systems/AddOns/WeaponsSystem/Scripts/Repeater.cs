using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Implements a single, burst or auto action.
    /// </summary>
    public class Repeater : MonoBehaviour
    {

        public enum RepeaterMode
        {
            Single,
            Burst,
            Automatic
        }

        [Header("Settings")]

        [Tooltip("The firing mode for this repeater.")]
        [SerializeField]
        protected RepeaterMode mode = RepeaterMode.Automatic;

        [Tooltip("The interval between actions during automatic mode, or between actions during a burst.")]
        [SerializeField]
        protected float interval = 0.15f;

        protected bool triggering = false;
        protected bool firingCoroutineRunning = false;

        protected Coroutine firingCoroutine;

        [Header("Burst Fire")]

        [Tooltip("The number of actions in a burst.")]
        [SerializeField]
        protected int burstSize = 1;

        [Tooltip("Whether to repeat the burst while the triggering remains on.")]
        [SerializeField]
        protected bool repeatBurst = false;

        [Tooltip("The interval between bursts.")]
        [SerializeField]
        protected float burstInterval = 0.5f;

        [Header("Events")]

        public UnityEvent OnAction;
        

        /// <summary>
        /// Called to start triggering this Repeater.
        /// </summary>
        public void StartTriggering()
        {
            if (!gameObject.activeInHierarchy) return;

            if (triggering) return;

            triggering = true;

            if (firingCoroutineRunning) return;

            
            switch (mode)
            {
                case RepeaterMode.Single:
                    Action();
                    break;
                case RepeaterMode.Burst:
                    firingCoroutine = StartCoroutine(Burst(burstSize));
                    break;
                case RepeaterMode.Automatic:
                    firingCoroutine = StartCoroutine(Automatic());
                    break;
            }
        }

        /// <summary>
        /// Called to stop triggering this Repeater.
        /// </summary>
        public void StopTriggering()
        {
            triggering = false;
        }


        /// <summary>
        /// Trigger this Repeater once.
        /// </summary>
        public void TriggerOnce()
        {

            if (!gameObject.activeSelf) return;

            switch (mode)
            {
                case RepeaterMode.Single:
                    Action();
                    break;
                case RepeaterMode.Burst:
                    firingCoroutine = StartCoroutine(Burst(burstSize));
                    break;
                case RepeaterMode.Automatic:
                    firingCoroutine = StartCoroutine(Automatic());
                    break;
            }
            
        }

        // The burst coroutine.
        IEnumerator Burst(int num)
        {
            firingCoroutineRunning = true;

            for(int i = 0; i < num; ++i)
            {
                Action();
                yield return new WaitForSeconds(interval);
            }

            if (repeatBurst)
            {
                yield return new WaitForSeconds(burstInterval);
                if (triggering)
                {
                    firingCoroutine = StartCoroutine(Burst(num));
                }
                else
                {
                    firingCoroutineRunning = false;
                }
            }
            else
            {
                firingCoroutineRunning = false;
            }
        }

        // Auto coroutine
        IEnumerator Automatic()
        {
            firingCoroutineRunning = true;

            while (triggering)
            {
                Action();
                yield return new WaitForSeconds(interval);
            }

            firingCoroutineRunning = false;
        }

        // Called to implement a single action.
        protected void Action()
        {
            OnAction.Invoke();
        }
    }
}

