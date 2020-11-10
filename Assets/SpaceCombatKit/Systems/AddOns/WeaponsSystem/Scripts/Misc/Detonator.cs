using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.Pooling;


namespace VSX.UniversalVehicleCombat
{

    public enum DetonationState
    {
        Reset,
        Detonating,
        Detonated
    }

    /// <summary>
    /// Unity event for running functions when a detonator is detonated.
    /// </summary>
    [System.Serializable]
    public class DetonationEventHandler : UnityEvent { }

  
    /// <summary>
    /// This class detonates an object (creates an explosion and deactivates the gameobject etc).
    /// </summary>
    public class Detonator : MonoBehaviour
    {
        protected DetonationState detonationState;
        public DetonationState DetonationState { get { return detonationState; } }

        protected Vector3 detonationPosition;
        protected Quaternion detonationRotation;

        [SerializeField]
        protected bool usePoolManager;

        [Header("Detonating")]

        [SerializeField]
        protected List<GameObject> detonatingStateSpawnObjects = new List<GameObject>();

        [SerializeField]
        protected float detonatingDuration = 0;

        [Header("Detonated")]

        [SerializeField]
        protected List<GameObject> detonatedStateSpawnObjects = new List<GameObject>();

        [Header("Timed Detonation")]

        [SerializeField]
        protected bool detonateAfterLifetime = false;

        [SerializeField]
        protected float lifeTime = 1;

        protected float lifeTimeStartTime;

        [Header("Events")]

        public DetonationEventHandler onDetonating;

        // Detonator detonated event
        public DetonationEventHandler onDetonated;    

        // Detonator reset event
        public DetonationEventHandler onReset;



        protected virtual void Start()
        {
            if (usePoolManager && PoolManager.Instance == null)
            {
                usePoolManager = false;
                Debug.LogWarning("Cannot pool explosions or hit effects as there isn't a PoolManager in the scene. Please add one to use pooling, or set the usePoolManager field on this component to False.");
            }
        }

        protected void SetDetonationState(DetonationState newState)
        {
            switch (newState)
            {
                case DetonationState.Detonating:

                    // Spawn Objects
                    for (int i = 0; i < detonatingStateSpawnObjects.Count; ++i)
                    {
                        if (usePoolManager)
                        {
                            PoolManager.Instance.Get(detonatingStateSpawnObjects[i], detonationPosition, detonationRotation);
                        }
                        else
                        {
                            GameObject.Instantiate(detonatingStateSpawnObjects[i], detonationPosition, detonationRotation);
                        }
                    }

                    // Call the event
                    onDetonating.Invoke();
                    break;

                case DetonationState.Detonated:

                    // Spawn Objects
                    for (int i = 0; i < detonatedStateSpawnObjects.Count; ++i)
                    {
                        if (usePoolManager)
                        {
                            PoolManager.Instance.Get(detonatedStateSpawnObjects[i], detonationPosition, detonationRotation);
                        }
                        else
                        {
                            GameObject.Instantiate(detonatedStateSpawnObjects[i], detonationPosition, detonationRotation);
                        }
                    }

                    // Call the event
                    onDetonated.Invoke();
                    break;

                case DetonationState.Reset:

                    // Call the event
                    onReset.Invoke();

                    break;
            }

            // Update the state
            detonationState = newState;
        }

        public virtual void BeginDelayedDetonation(float delay)
        {
            StartCoroutine(DelayedDetonation(delay));
        }

        protected IEnumerator DelayedDetonation(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (detonationState == DetonationState.Reset)
            {
                Detonate();
            }
        }

        // Coroutine for detonation
        protected IEnumerator DetonationCoroutine()
        {
            SetDetonationState(DetonationState.Detonating);
            yield return new WaitForSeconds(detonatingDuration);
            SetDetonationState(DetonationState.Detonated);
        }

        /// <summary>
        /// Detonate at the current position.
        /// </summary>
	    public virtual void Detonate()
        {
            Detonate(transform.position, transform.up);
        }

        /// <summary>
        /// Detonate at a raycast hit point.
        /// </summary>
        /// <param name="hit">The raycast hit information.</param>
        public virtual void Detonate(RaycastHit hit)
        {
            Detonate(hit.point, hit.normal);
        }

        /// <summary>
        /// Detonate at a world position.
        /// </summary>
        /// <param name="detonationPosition">The detonation position.</param>
        public virtual void Detonate(Vector3 detonationPosition, Vector3 detonationForward)
        {

            this.detonationPosition = detonationPosition;
            this.detonationRotation = Quaternion.LookRotation(detonationForward);

            if (detonationState != DetonationState.Reset) return;

            transform.position = detonationPosition;

            // Start the coroutine
            if (gameObject.activeInHierarchy) StartCoroutine(DetonationCoroutine());
            
        }


        /// <summary>
        /// Reset the detonator.
        /// </summary>
        public virtual void ResetDetonator()
        {
            SetDetonationState(DetonationState.Reset);
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
        }

        protected virtual void OnEnable()
        {
            ResetDetonator();
            lifeTimeStartTime = Time.time;
        }

        protected virtual void Update()
        {
            if (detonateAfterLifetime)
            {
                if (Time.time - lifeTimeStartTime > lifeTime)
                {
                    Detonate(transform.position, transform.up);
                }
            }
        }
    }
}