using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using UnityEngine.Events;
using VSX.Pooling;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Unity event for running functions when a projectile is launched by a projectile launcher
    /// </summary>
    [System.Serializable]
    public class OnProjectileLauncherProjectileLaunchedEventHandler : UnityEvent<GameObject> { }
    
    /// <summary>
    /// This class spawns a projectile prefab at a specified interval and with a specified launch velocity.
    /// </summary>
    public class ProjectileLauncher : MonoBehaviour, IRootTransformUser
    {

        [Header("Settings")]

        [SerializeField]
        protected Transform spawnPoint;

        [SerializeField]
        protected GameObject projectilePrefab;
        public GameObject ProjectilePrefab { get { return projectilePrefab; } }

        [SerializeField]
        protected bool usePoolManager;

        [SerializeField]
        protected bool addLauncherVelocityToProjectile;


        [Header("Events")]

        // Projectile launched event
        public OnProjectileLauncherProjectileLaunchedEventHandler onProjectileLaunched;

        protected Transform rootTransform;
        public Transform RootTransform
        {
            set
            {
                rootTransform = value;
                if (rootTransform != null)
                {
                    rBody = rootTransform.GetComponent<Rigidbody>();
                }
                else
                {
                    rBody = null;
                }
            }
        }

        protected Rigidbody rBody;



        protected virtual void Reset()
        {
            spawnPoint = transform;
        }


        protected virtual void Awake()
        {

            if (rootTransform == null) rootTransform = transform.root;

            if (rootTransform != null)
            {
                rBody = rootTransform.GetComponent<Rigidbody>();
            }
        }

        protected virtual void Start()
        {
            if (usePoolManager && PoolManager.Instance == null)
            {
                usePoolManager = false;
                Debug.LogWarning("No PoolManager component found in scene, please add one to pool projectiles.");
            }
        }

        // Launch a projectile
        public virtual void LaunchProjectile()
        {
            if (projectilePrefab != null)
            {
                // Get/instantiate the projectile
                GameObject projectileObject;

                if (usePoolManager)
                {
                    projectileObject = PoolManager.Instance.Get(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
                }
                else
                {
                    projectileObject = GameObject.Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
                }

                Projectile projectileController = projectileObject.GetComponent<Projectile>();
                if (projectileController != null)
                {
                    projectileController.SetSenderRootTransform(rootTransform);

                    if (addLauncherVelocityToProjectile && rBody != null)
                    {
                        projectileController.AddVelocity(rBody.velocity);
                    }
                }

                // Call the event
                onProjectileLaunched.Invoke(projectileObject);
            }
        }
    }
}