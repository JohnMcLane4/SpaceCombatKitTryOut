using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Unity event for running functions related to an individual wave
    /// </summary>
    [System.Serializable]
    public class WaveEventHandler : UnityEvent { }

    /// <summary>
    /// Manages a wave made up of WaveSpawn objects.
    /// </summary>
    public class WaveController : MonoBehaviour
    {

        [Header("General")]

        [SerializeField]
        protected List<PilotedVehicleSpawn> spawners = new List<PilotedVehicleSpawn>();

        [SerializeField]
        protected Transform spawnsParent;

        [SerializeField]
        protected bool spawnOnEnable = false;

        protected bool spawningWave = false;

        [Header("Spawn Interval")]

        [SerializeField]
        protected float minSpawnInterval = 0.25f;

        [SerializeField]
        protected float maxSpawnInterval = 0.5f;

        protected int nextSpawnIndex = 0;
        protected float delayStartTime;
        protected float nextSpawnInterval;

        [Header("Camera View Spawning")]

        [SerializeField]
        protected bool spawnInCameraView = true;

        [SerializeField]
        protected Camera viewCamera;

        [SerializeField]
        protected float spawnDistanceFromCamera = 250;

        protected bool waveDestroyed = false;
        public bool WaveDestroyed { get { return waveDestroyed; } }

        [Header("Events")]

        public WaveEventHandler onWaveDestroyed;


        protected virtual void Awake()
        {
            if (viewCamera == null)
            {
                viewCamera = Camera.main;
            }
        }

        // Called when the component is first added to a gameobject, or when it is reset in the inspector
        protected virtual void Reset()
        {
            spawnsParent = transform;
        }


        // Update the positions of the spawn points to be in camera view
        protected virtual void PositionSpawnsInCameraView()
        {
            // Check if conditions exist to run this function
            if (!spawnInCameraView || viewCamera == null || spawners.Count == 0) return;

            // Calculate the spawn position for the wave
            Vector3 spawnPos = viewCamera.transform.position + viewCamera.transform.forward * spawnDistanceFromCamera;

            // Get the vertically flattened camera forward vector and turn it around to get the way the wave will be facing.
            Vector3 cameraFlattened = viewCamera.transform.forward;
            cameraFlattened.y = 0;
            cameraFlattened.Normalize();
            if (cameraFlattened.magnitude < 0.01f)
            {
                cameraFlattened = Vector3.forward;
            }

            // Give the spawn facing direction a randomization relative to the camera
            Vector3 spawnPosDir = Quaternion.Euler(0f, Random.Range(-90, 90), 0f) * -cameraFlattened;

            // Position and rotate the spawn parent
            spawnsParent.position = spawnPos;
            spawnsParent.rotation = Quaternion.LookRotation(spawnPosDir, Vector3.up);

        }


        protected virtual void OnEnable()
        {
            if (spawnOnEnable)
            {
                Spawn();
            }
        }

        /// <summary>
        /// Spawn the wave.
        /// </summary>
        public virtual void Spawn()
        {
            PositionSpawnsInCameraView();
            waveDestroyed = false;
            nextSpawnIndex = 0;
            spawningWave = true;
            GetSpawnInterval();
        }

        // Get a new spawn interval.
        protected virtual void GetSpawnInterval()
        {
            delayStartTime = Time.time;
            nextSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        /// <summary>
        /// Spawn a specific item in the wave according to the list index.
        /// </summary>
        /// <param name="listIndex">The list index of the item to spawn.</param>
        public virtual void Spawn(int listIndex)
        {
            waveDestroyed = false;
            spawners[listIndex].Spawn();
        }

        // Called every frame
        protected virtual void Update()
        {
            // If the wave is spawning, calculate if the required interval has paseds
            if (spawningWave)
            {
                if (nextSpawnIndex < spawners.Count)
                {
                    // If the required interval has passed, spawn the next object
                    if (Time.time - delayStartTime >= nextSpawnInterval)
                    {
                        Spawn(nextSpawnIndex);
                        nextSpawnIndex += 1;
                        GetSpawnInterval();
                    }
                }
                else
                {
                    spawningWave = false;
                }
            }

            // Check if all the waves have been destroyed to call the event.
            if (!spawningWave && !waveDestroyed)
            {
                bool check = true;
                for (int i = 0; i < spawners.Count; ++i)
                {
                    if (!spawners[i].Destroyed)
                    {
                        check = false;
                        break;
                    }
                }
                
                if (check)
                {
                    waveDestroyed = true;
                    onWaveDestroyed.Invoke();
                }
            }
        }
    }
}
