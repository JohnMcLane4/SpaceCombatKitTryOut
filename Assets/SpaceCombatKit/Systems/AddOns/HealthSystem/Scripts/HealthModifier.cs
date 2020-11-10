using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Emits damage to damage receivers, either at a raycast hit point, or to all damage receivers in the vicinity.
    /// </summary>
    public class HealthModifier : MonoBehaviour, IRootTransformUser
    {

        [Header("Damage")]

        [SerializeField]
        protected float defaultDamageValue = 100;
        public float DefaultDamageValue { get { return defaultDamageValue; } }

        [SerializeField]
        protected List<HealthModifierValue> damageOverrideValues = new List<HealthModifierValue>();
        public List<HealthModifierValue> DamageOverrideValues { get { return damageOverrideValues; } }

        [SerializeField]
        protected AnimationCurve damageFalloffCurve = AnimationCurve.Linear(0, 1, 1, 1);

        [Header("Healing")]

        [SerializeField]
        protected float defaultHealingValue = 0;
        public float DefaultHealingValue { get { return defaultHealingValue; } }

        [SerializeField]
        protected List<HealthModifierValue> healingOverrideValues = new List<HealthModifierValue>();
        public List<HealthModifierValue> HealingOverrideValues { get { return healingOverrideValues; } }

        [SerializeField]
        protected AnimationCurve healingFalloffCurve = AnimationCurve.Linear(0, 1, 1, 1);

        [Header("General")]

        [SerializeField]
        protected HealthModifierType healthModifierType;

        // Whether to apply the damage amount on a per-second basis (e.g. for laser beams).
        [SerializeField]
        protected bool timeBased = false;

        [SerializeField]
        protected float maxEffectDistance = 2000;

        protected float effectMultiplier;

        [SerializeField]
        protected Transform rootTransform;
        public Transform RootTransform
        {
            set { rootTransform = value; }
        }

        [Header("Area Damage")]

        [SerializeField]
        protected DamageReceiverScanner areaDamageReceiverScanner;

        


        /// <summary>
        /// Called when the component is first added to a gameobject, or reset in the inspector.
        /// </summary>
        protected virtual void Reset()
        {
            rootTransform = transform;
#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
            if (UnityEditor.PrefabUtility.GetPrefabAssetType(transform.root) != UnityEditor.PrefabAssetType.NotAPrefab)
            {
                rootTransform = transform.root;
            }
#else
                    if (UnityEditor.PrefabUtility.GetPrefabType(transform.root) != UnityEditor.PrefabType.None) {
                        rootTransform = transform.root;
                    }
#endif
#endif
        }


        public void SetEffectMultiplier(float multiplier)
        {
            this.effectMultiplier = multiplier;
        }


        /// <summary>
        /// Emit damage based on a raycast hit.
        /// </summary>
        /// <param name="hit">The raycast hit information.</param>
        public virtual void RaycastHitDamage(RaycastHit hit)
        {
            DamageReceiver damageReceiver = hit.collider.GetComponent<DamageReceiver>();

            if (damageReceiver != null)
            {
                // Don't apply damage or healing to self
                if (damageReceiver.RootTransform == rootTransform) return;

                // Do damage
                float damageValue = defaultDamageValue * (timeBased ? Time.deltaTime : 1);
                for (int i = 0; i < damageOverrideValues.Count; ++i)
                {
                    if (damageOverrideValues[i].HealthType == damageReceiver.HealthType)
                    {

                        float damageAmount = damageFalloffCurve.Evaluate(Vector3.Distance(transform.position, hit.point) / maxEffectDistance);

                        damageValue = damageOverrideValues[i].Value * damageAmount;

                        if (timeBased)
                        {
                            damageValue *= Time.deltaTime;
                        }
                    }
                }

                if (damageValue != 0)
                {
                    damageReceiver.Damage(damageValue, hit.point, healthModifierType, rootTransform);
                }

                // Calculate healing value
                float healingValue = defaultHealingValue * (timeBased ? Time.deltaTime : 1);
                for (int i = 0; i < healingOverrideValues.Count; ++i)
                {
                    if (healingOverrideValues[i].HealthType == damageReceiver.HealthType)
                    {

                        float healingAmount = damageFalloffCurve.Evaluate(Vector3.Distance(transform.position, hit.point) / maxEffectDistance);

                        float healing = healingOverrideValues[i].Value * healingAmount;

                        if (timeBased)
                        {
                            healing *= Time.deltaTime;
                        }
                    }
                }

                // Apply healing
                if (healingValue != 0)
                {
                    damageReceiver.Heal(healingValue, hit.point, healthModifierType, rootTransform);
                }
            }
        }

        public void RaycastHitAreaDamage(RaycastHit hit)
        {
            EmitDamageFromPoint(hit.point);
        }


        /// <summary>
        /// Emit damage from the current position to any damage receivers in range.
        /// </summary>
        public void EmitDamage()
        {
            // Damage all the damageables that are inside the damageable scanner's trigger collider
            EmitDamageFromPoint(transform.position);
        }


        /// <summary>
        /// Emit damage from a point in world space to any damage receivers in range.
        /// </summary>
        /// <param name="damageEmissionPoint">The world space point from which to emit damage.</param>
        public void EmitDamageFromPoint(Vector3 damageEmissionPoint)
        {
            if (areaDamageReceiverScanner != null)
            {
                // Go through all the detected damageables and damage them according to the damage falloff parameters
                for (int i = 0; i < areaDamageReceiverScanner.DamageReceiversInRange.Count; ++i)
                {

                    if (areaDamageReceiverScanner.DamageReceiversInRange[i].RootTransform == rootTransform.gameObject) return;

                    // Get the distance from the damage emitter to the damageable's closest point
                    Vector3 closestPoint = areaDamageReceiverScanner.DamageReceiversInRange[i].GetClosestPoint(damageEmissionPoint);
                    float distance = Vector3.Distance(damageEmissionPoint, closestPoint);

                    // Damage

                    // Emit each type of damage this damage emitter can emit, based on falloff curve
                    float damageValue = defaultDamageValue * (timeBased ? Time.deltaTime : 1);
                    for (int j = 0; j < damageOverrideValues.Count; ++j)
                    {
                        if (damageOverrideValues[j].HealthType == areaDamageReceiverScanner.DamageReceiversInRange[i].HealthType)
                        {
                            damageValue = damageFalloffCurve.Evaluate(distance / areaDamageReceiverScanner.ScannerTriggerCollider.radius) * damageOverrideValues[j].Value;

                            if (timeBased)
                            {
                                damageValue *= Time.deltaTime;
                            }

                            break;
                        }
                    }

                    if (damageValue != 0)
                    {
                        areaDamageReceiverScanner.DamageReceiversInRange[i].Damage(damageValue, closestPoint, healthModifierType, rootTransform);

                    }


                    // Emit each type of healing this damage emitter can emit, based on falloff curve
                    float healingValue = defaultHealingValue * (timeBased ? Time.deltaTime : 1);
                    for (int j = 0; j < healingOverrideValues.Count; ++j)
                    {
                        if (healingOverrideValues[j].HealthType == areaDamageReceiverScanner.DamageReceiversInRange[i].HealthType)
                        {
                            float healing = healingFalloffCurve.Evaluate(distance / areaDamageReceiverScanner.ScannerTriggerCollider.radius) * healingOverrideValues[j].Value;

                            if (timeBased)
                            {
                                healing *= Time.deltaTime;
                            }

                            
                            break;
                        }
                    }

                    if (healingValue != 0)
                    {
                        areaDamageReceiverScanner.DamageReceiversInRange[i].Heal(healingValue, closestPoint, healthModifierType, rootTransform);
                    }
                }
            }
        }
    }
}