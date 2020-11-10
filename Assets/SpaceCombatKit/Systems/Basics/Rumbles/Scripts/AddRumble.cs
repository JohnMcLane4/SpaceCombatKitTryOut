using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class AddRumble : MonoBehaviour
    {

        [Header("Settings")]

        [SerializeField]
        protected bool runOnEnable = true;

        [Header("Rumble Parameters")]

        [SerializeField]
        protected bool distanceBased = true;

        [SerializeField]
        protected float defaultDelay = 0;

        [SerializeField]
        protected float maxRumbleLevel = 1;

        [SerializeField]
        protected float attackTime = 0.05f;

        [SerializeField]
        protected float sustainTime = 0.2f;

        [SerializeField]
        protected float decayTime = 0.6f;

        

        private void OnEnable()
        {
            if (runOnEnable)
            {
                Run();
            }
        }

        public void Run()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(RunCoroutine(defaultDelay));
            }
        }

        public void Run(float delay)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(RunCoroutine(delay));
            }
        }

        protected IEnumerator RunCoroutine(float delay)
        {

            yield return new WaitForSeconds(delay);

            // Add a rumble
            if (RumbleManager.Instance != null)
            {
                RumbleManager.Instance.AddRumble(distanceBased, transform.position, maxRumbleLevel, attackTime, sustainTime, decayTime);
            }
        }
    }
}