using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Unity event for running functions related to a set of waves.
    /// </summary>
    [System.Serializable]
    public class WavesEventHandler : UnityEvent { }

    /// <summary>
    /// Manage a set of waves.
    /// </summary>
    public class WavesController : MonoBehaviour
    {
        [Header("General")]

        [SerializeField]
        protected List<WaveController> waveControllers = new List<WaveController>();

        [SerializeField]
        protected bool loopWaves = false;

        protected int nextWaveSpawnIndex = 0;

        protected bool wavesDestroyed = false;

        [Header("Events")]

        public WavesEventHandler onWavesDestroyed;


        /// <summary>
        /// Spawn a wave at a specific index in the list
        /// </summary>
        /// <param name="index">The wave index to spawn.</param>
        public virtual void SpawnWave(int index)
        {
            if (index == 0 || index >= waveControllers.Count) return;
            waveControllers[index].Spawn();
        }

        /// <summary>
        /// Spawn a random wave in the list.
        /// </summary>
        public virtual void SpawnRandomWave()
        {
            if (waveControllers.Count == 0) return;
            waveControllers[Random.Range(0, waveControllers.Count)].Spawn();
        }

        /// <summary>
        /// Spawn the next wave in the list.
        /// </summary>
        public virtual void SpawnNextWave()
        {
            if (waveControllers.Count == 0 || nextWaveSpawnIndex >= waveControllers.Count) return;

            waveControllers[nextWaveSpawnIndex].Spawn();

            // Iterate
            nextWaveSpawnIndex += 1;
            if (nextWaveSpawnIndex >= waveControllers.Count)
            {
                if (loopWaves)
                {
                    nextWaveSpawnIndex = 0;
                }
                else
                {
                    return;
                }
            }
        }

        // Called every frame
        protected virtual void Update()
        {
            // Check if all the waves have been destroyed
            if (!wavesDestroyed)
            {
                wavesDestroyed = true;
                for (int i = 0; i < waveControllers.Count; ++i)
                {
                    if (!waveControllers[i].WaveDestroyed)
                    {
                        wavesDestroyed = false;
                    }
                }

                if (wavesDestroyed)
                {
                    onWavesDestroyed.Invoke();
                }
            }
        }
    }
}

