using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Unity event for running functions when a child vehicle enters a parent vehicle.
    /// </summary>
    [System.Serializable]
    public class OnChildVehicleEnteredParentEventHandler : UnityEvent { }

    /// <summary>
    /// Unity event for running functions when a child vehicle exits a parent vehicle.
    /// </summary>
    [System.Serializable]
    public class OnChildVehicleExitedParentEventHandler : UnityEvent { }

    [RequireComponent(typeof(Vehicle))]
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleEnterExitManager : MonoBehaviour
    {

        // Child

        [SerializeField]
        protected VehicleEnterExitManager startingChildVehicle;

        protected VehicleEnterExitManager child;
        public VehicleEnterExitManager Child
        {
            get { return child; }
        }

        protected VehicleEnterExitManager parent;
        public VehicleEnterExitManager Parent { get { return parent; } }

        [SerializeField]
        protected Transform spawnPoint;
        public Transform SpawnPoint { get { return spawnPoint; } }

        // Parent

        protected List<VehicleEnterExitManager> enterableVehicles = new List<VehicleEnterExitManager>();
        public List<VehicleEnterExitManager> EnterableVehicles { get { return enterableVehicles; } }

        protected Vehicle vehicle;
        public Vehicle Vehicle { get { return vehicle; } }

        [SerializeField]
        protected List<VehicleClass> enterableVehicleClasses = new List<VehicleClass>(); // the vehicle classes this vehicle can enter

        [SerializeField]
        protected bool disableVehicleOnEnterParent = true;

        [Header("Events")]

        public OnChildVehicleEnteredParentEventHandler onChildEntered;
        public OnChildVehicleExitedParentEventHandler onChildExited;

        public OnChildVehicleEnteredParentEventHandler onEnteredParent;
        public OnChildVehicleExitedParentEventHandler onExitedParent;


        protected virtual void Reset()
        {
            vehicle = transform.GetComponent<Vehicle>();
        }


        protected virtual void Awake()
        {
            vehicle = GetComponent<Vehicle>();
            if (vehicle == null)
            {
                Debug.LogError("Vehicle Enter Exit Manager component requires a reference to the vehicle, please set this in the inspector.");
            }

            child = startingChildVehicle;
        }
        
        public virtual void AddEnterableVehicle(VehicleEnterExitManager enterableVehicle)
        {
            if (enterableVehicle == null) return;

            if (enterableVehicles.Contains(enterableVehicle)) return;

            if (enterableVehicleClasses.Contains(enterableVehicle.Vehicle.VehicleClass))
            {
                enterableVehicles.Add(enterableVehicle);
            }            
        }

        public virtual void RemoveEnterableVehicle(VehicleEnterExitManager enterableVehicle)
        {
            if (enterableVehicle == null) return;

            if (enterableVehicles.Contains(enterableVehicle))
            {
                enterableVehicles.Remove(enterableVehicle);
            }
        }

        public virtual bool CanEnter(VehicleEnterExitManager vehicleEnterExitManager)
        {
            return (enterableVehicleClasses.Contains(vehicleEnterExitManager.Vehicle.VehicleClass));
        }

        public virtual void SetChild(VehicleEnterExitManager child)
        {
            this.child = child;
        }

        public virtual void EnterParent(int index = 0)
        {
            if (enterableVehicles.Count > index)
            {
                parent = enterableVehicles[index];
                parent.OnChildEntered(this);
                gameObject.SetActive(false);
                onEnteredParent.Invoke();

                if (disableVehicleOnEnterParent)
                {
                    vehicle.gameObject.SetActive(false);
                }
            }
        }

        public virtual void OnChildEntered(VehicleEnterExitManager child)
        {
            this.child = child;

            onChildEntered.Invoke();
        }


        // Parent active
        public virtual bool CanExitToChild()
        {
            return (child != null);
        }

        public virtual void ExitToChild()
        {
            
            enterableVehicles.Clear();
            child.transform.position = spawnPoint.position;
            child.transform.rotation = spawnPoint.rotation;
            child.OnExitedParent();
            
            onChildExited.Invoke();
        }

        public virtual void OnExitedParent()
        {
            gameObject.SetActive(true);
            onExitedParent.Invoke();
        }


        /// <summary>
        /// Called every frame that a collider is inside a trigger collider.
        /// </summary>
        /// <param name="other">The collider that is inside the trigger collider.</param>
        protected virtual void OnTriggerEnter(Collider other)
        {

            if (other.attachedRigidbody == null) return;

            // Get other's enter exit manager
            VehicleEnterExitManager otherEnterExitManager = other.attachedRigidbody.GetComponent<VehicleEnterExitManager>();

            // Set enterable vehicle
            if (otherEnterExitManager != null)
            {
                otherEnterExitManager.AddEnterableVehicle(this);
            }
        }

        /// <summary>
        /// Called when a collider exits a trigger collider.
        /// </summary>
        /// <param name="other">The collider that exited.</param>
        protected virtual void OnTriggerExit(Collider other)
        {

            if (other.attachedRigidbody == null) return;

            // Get other's enter exit manager
            VehicleEnterExitManager otherEnterExitManager = other.attachedRigidbody.GetComponent<VehicleEnterExitManager>();

            // Unset enterable vehicle
            if (otherEnterExitManager != null)
            {
                otherEnterExitManager.RemoveEnterableVehicle(this);
            }
        }
    }
}
