using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{ 
    // Unity event for running functions when the triggerable starts being triggered
    [System.Serializable]
    public class OnStartTriggeringEventHandler : UnityEvent { }

    // Unity event for running functions when the triggerable stops being triggered
    [System.Serializable]
    public class OnStopTriggeringEventHandler : UnityEvent { }

    // Unity event for running functions when the triggerable is triggered once
    [System.Serializable]
    public class OnTriggerOnceEventHandler : UnityEvent { }

    // Unity event for running functions when the triggerable's trigger level is set
    [System.Serializable]
    public class OnSetTriggerLevelEventHandler : UnityEvent <float> { }


    /// <summary>
    /// Provides a way to make a module triggerable by connecting trigger inputs to actions.
    /// </summary>
    public class Triggerable : MonoBehaviour
    {

        [Header("General")]

        [Tooltip("Whether this Triggerable is managed by the TriggerablesManager on a vehicle (set to false when using an auto turret).")]
        [SerializeField]
        protected bool managedLocally = false;
        public bool ManagedLocally { get { return managedLocally; } }

        [Tooltip("The default trigger index to which this Triggerable is assigned.")]
        [SerializeField]
        protected int defaultTrigger = 0;
        public int DefaultTrigger { get { return defaultTrigger; } }

        [Tooltip("The ordering index used to determine the order of items when shown in a list or menu.")]
        [SerializeField]
        protected int orderingIndex = 0;
        public int OrderingIndex { get { return orderingIndex; } }

        [Header("Events")]

        // Start triggering event
        public OnStartTriggeringEventHandler onStartTriggering;

        // Stop triggering event
        public OnStopTriggeringEventHandler onStopTriggering;

        // Trigger once event
        public OnTriggerOnceEventHandler onTriggerOnce;

        // Set trigger level event
        public OnSetTriggerLevelEventHandler onSetTriggerLevel;


        /// <summary>
        /// Start triggering the triggerable.
        /// </summary>
	    public virtual void StartTriggering()
        {
            onStartTriggering.Invoke();
        }

        /// <summary>
        /// Stop triggering the triggerable.
        /// </summary>
        public virtual void StopTriggering()
        {
            onStopTriggering.Invoke();
        }

        /// <summary>
        /// Trigger the triggerable once.
        /// </summary>
        public virtual void TriggerOnce()
        {
            onTriggerOnce.Invoke();
        }

        /// <summary>
        /// Set the triggerable's trigger level.
        /// </summary>
        /// <param name="level"></param>
        public virtual void SetTriggerLevel(float level)
        {
            onSetTriggerLevel.Invoke(level);
        }
    }
}