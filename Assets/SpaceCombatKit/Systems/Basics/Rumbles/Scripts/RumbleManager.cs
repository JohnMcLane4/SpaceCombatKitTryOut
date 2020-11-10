using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{
    [System.Serializable]
    public class OnRumbleEventHandler : UnityEvent<float> { }

    /// <summary>
    /// This class is used to store information about a single instance of a rumble.
    /// </summary>
    [System.Serializable]
    public class Rumble
    {

        public bool distanceBased = true;

        // The position of the rumble
        public Vector3 position;

        // The strength of the rumble
        public float maxLevel;

        // How long it takes to reach maximum strength
        public float attackTime;

        // How long it holds at maximum strength
        public float sustainTime;

        // How long it takes to fade away
        public float decayTime;

        // The time that the rumble began
        [HideInInspector]
        public float startTime;

        public Rumble(bool distanceBased, Vector3 position, float maxLevel, float attackTime, float sustainTime, float decayTime)
        {

            this.distanceBased = distanceBased;

            this.position = position;

            this.maxLevel = maxLevel;

            this.attackTime = attackTime;
            this.sustainTime = sustainTime;
            this.decayTime = decayTime;

            this.startTime = Time.time;
        }
    }


    /// <summary>
    /// This class provides a way the create 'rumbles' based on things like being hit, colliding, and boosting, which 
    /// can be used to drive camera shaking, controller vibration etc.
    /// </summary>
    public class RumbleManager : MonoBehaviour
    {

        public Transform player;
        public float maxEffectDistance = 1000;
        public AnimationCurve rumbleDistanceFalloff = AnimationCurve.Linear(0, 0, 1, 1);

        // All of the rumbles currently executing.
        protected List<Rumble> rumbles = new List<Rumble>();

        // The current rumble level (max of all rumbles)
        protected float currentLevel = 0;
        public float CurrentLevel { get { return currentLevel; } }

        // The current single-frame rumble strength
        protected float currentSingleFrameRumbleStrength = 0;

        public OnRumbleEventHandler onRumble;

        public static RumbleManager Instance;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public void SetPlayerVehicle(Vehicle vehicle)
        {
            if (vehicle != null)
            {
                player = vehicle.transform;
            }
            else
            {
                player = null;
            }
        }

     
        /// <summary>
        /// Add a new rumble.
        /// </summary>
        /// <param name="maxLevel">The shake strength.</param>
        /// <param name="attackTime">How fast the camera shake builds up.</param>
        /// <param name="sustainTime">How long the shake sustains at maximum strength.</param>
        /// <param name="decayTime">How long the shake takes to decay/disappear.</param>
        public void AddRumble(bool distanceBased, Vector3 position, float maxLevel, float attackTime, float sustainTime, float decayTime)
        {
            // Add a new rumble
            Rumble newRumble = new Rumble(distanceBased, position, maxLevel, attackTime, sustainTime, decayTime);
            rumbles.Add(newRumble);
        }


        /// <summary>
        /// Add a new rumble for a single frame.
        /// </summary>
        /// <param name="singleRumbleLevel">The single frame rumble level.</param>
        public void AddSingleFrameRumble(float singleFrameRumbleLevel)
        {
            currentSingleFrameRumbleStrength = Mathf.Max(currentSingleFrameRumbleStrength, singleFrameRumbleLevel);
        }


        public void Shake(float level)
        {
            // Do a single shake
            // Get a random vector on the xy plane
            Vector3 shakeVector = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), 0f).normalized;

            // Scale according to desired shake magnitude
            shakeVector *= level * 0.01f;

            // Look at shake vector
            Vector3 lookToVector = transform.forward + shakeVector;
            transform.rotation = Quaternion.LookRotation(lookToVector, transform.up);

        }


        // Called every frame
        void Update()
        {
           
            // Update the current rumble level (max of all rumbles).
            currentLevel = 0;
            for (int i = 0; i < rumbles.Count; ++i)
            {

                float timeSinceStart = Time.time - rumbles[i].startTime;

                // If the rumble is finished, remove it.
                if (timeSinceStart > (rumbles[i].attackTime + rumbles[i].sustainTime + rumbles[i].decayTime))
                {
                    rumbles.RemoveAt(i);
                    i--;
                    continue;
                }

                float level = 0;
                
                // Get the current level of the rumble.
                if (timeSinceStart < rumbles[i].attackTime)
                {
                    level = timeSinceStart / rumbles[i].attackTime;
                }
                else if (timeSinceStart < rumbles[i].attackTime + rumbles[i].sustainTime)
                {
                    level = 1;
                }
                else
                {
                    float timeSinceBeganDecay = timeSinceStart - rumbles[i].attackTime - rumbles[i].sustainTime;
                    level = Mathf.Clamp(1 - timeSinceBeganDecay / rumbles[i].decayTime, 0f, 1f);
                }

                float distFromPlayer = (rumbles[i].distanceBased && player != null) ? Vector3.Distance(player.position, rumbles[i].position) : 0;
                float distanceMultiplier = maxEffectDistance == 0 ? 0 : rumbleDistanceFalloff.Evaluate(distFromPlayer / maxEffectDistance);
                
                // Update the current level if this is the biggest rumble
                currentLevel = Mathf.Max(currentLevel, level * rumbles[i].maxLevel * distanceMultiplier);
            }
            
            // Update the current level with the single shake level
            currentLevel = Mathf.Max(currentLevel, currentSingleFrameRumbleStrength);

            if (currentLevel > 0.00001f)
            {
                onRumble.Invoke(currentLevel);
            }

            // Reset the single shake level
            currentSingleFrameRumbleStrength = 0;

        }
    }
}
