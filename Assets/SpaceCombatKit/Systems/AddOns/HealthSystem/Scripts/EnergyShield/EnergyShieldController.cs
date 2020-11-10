using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// This class manages the visual effects for an energy shield.
    /// </summary>
    public class EnergyShieldController : MonoBehaviour
    {

        [Header("General")]

        [Tooltip("How fast the shield effect fades away after a hit.")]
        [SerializeField]
        protected float effectFadeSpeed = 1f;

        [Tooltip("The distance below which effects are merged together into a single larger effect.")]
        [SerializeField]
        protected float mergeEffectDistance;  // Merge effects closer than this distance together

        [Tooltip("When there are too many effects, an effect with a value lower than this will be overriden to add a new one. Prevents almost-faded effects from taking up slots.")]
        [SerializeField]
        protected float overrideEffectThreshold = 0.01f;  // An effect slot can be used for a new effect when the strength is lower than this value

        [Tooltip("The mesh renderer for the shield effect.")]
        [SerializeField]
        protected MeshRenderer energyShieldMeshRenderer;
        public MeshRenderer EnergyShieldMeshRenderer
        {
            get { return energyShieldMeshRenderer; }
            set { energyShieldMeshRenderer = value; }
        }

        protected int numEffectSlots = 10;

        [Header("Damage Effects")]

        [Tooltip("Control the intensity of damage effects on the shield.")]
        [SerializeField]
        protected float damageEffectMultiplier = 0.1f;

        [Tooltip("The shield effect color for damage effects.")]
        [SerializeField]
        protected Color damageEffectColor = new Color(0.25f, 0.25f, 1f);

        [Header("Healing Effects")]

        [Tooltip("Control the intensity of healing effects on the shield.")]
        [SerializeField]
        protected float healEffectMultiplier = 0.1f;

        [Tooltip("The shield effect color for healing effects.")]
        [SerializeField]
        protected Color healEffectColor = new Color(1f, 0f, 0.5f);

        // This is a list of the hit effect data that is passed to the shader to carry out hit effects.
        // First three values correspond to the effect position (local to the shield mesh), fourth value is the effect strength.
        protected List<Vector4> effectPositions = new List<Vector4>();


        // Called when this component is first added to a gameobject or reset in inspector
        protected virtual void Reset()
        {
            energyShieldMeshRenderer = GetComponent<MeshRenderer>();
        }

        protected virtual void Awake()
        {
            // Initialize all of the effects to zero
            for (int i = 0; i < numEffectSlots; ++i)
            {
                effectPositions.Add(Vector4.zero);
            }
        }

        /// <summary>
        /// Called when the shield is damaged.
        /// </summary>
        /// <param name="damageValue">The amount of damage.</param>
        /// <param name="hitPosition">World space hit position.</param>
        public virtual void OnDamaged(float damageValue, Vector3 hitPosition, HealthModifierType healthModifierType, Transform damageSourceRootTransform)
        {
            ShowEffect(damageValue * damageEffectMultiplier, hitPosition, damageEffectColor);
        }

        /// <summary>
        /// Called when the shield is healed;
        /// </summary>
        /// <param name="healValue">The amount of healing.</param>
        /// <param name="hitPosition">World space hit position.</param>
        public virtual void OnHealed(float healValue, Vector3 hitPosition, HealthModifierType healthModifierType, Transform damageSourceRootTransform)
        {
            ShowEffect(healValue * healEffectMultiplier, hitPosition, healEffectColor);
        }

        /// <summary>
        /// Show an effect on the shield.
        /// </summary>
        /// <param name="effectStrength">The strength of the effect.</param>
        /// <param name="hitPosition">The world space hit position.</param>
        /// <param name="effectColor">The effect color.</param>
        public virtual void ShowEffect(float effectStrength, Vector3 hitPosition, Color effectColor)
        {

            if (energyShieldMeshRenderer == null) return;

            // Get the local hit position wrt the shield mesh
            Vector3 localHitPosition = transform.InverseTransformPoint(hitPosition);

            // Find an available effect slot and do the effect
            for (int i = 0; i < numEffectSlots; ++i)
            {
                // Get the position 
                Vector3 pos = new Vector3(effectPositions[i].x, effectPositions[i].y, effectPositions[i].z);
                bool isInMergeDistance = (Vector3.Distance(localHitPosition, pos) < mergeEffectDistance);

                if (isInMergeDistance || effectPositions[i].w < overrideEffectThreshold)
                {

                    // Set the values for the shader
                    Vector4 temp = new Vector4();
                    temp.w = Mathf.Max(effectPositions[i].w + effectStrength, 0.0001f);

                    temp.x = localHitPosition.x;
                    temp.y = localHitPosition.y;
                    temp.z = localHitPosition.z;

                    
                    Color c = energyShieldMeshRenderer.material.GetColor("_EffectColor" + i.ToString());
                    c = (effectPositions[i].w / temp.w) * c + (effectStrength / temp.w) * effectColor;
                    energyShieldMeshRenderer.material.SetColor("_EffectColor" + i.ToString(), c);
                    
                    effectPositions[i] = temp;
                    
                    break;
                }
            }
        }

        
        // Called every frame to update hit visual effects.
        protected virtual void UpdateEffects()
        {

            if (energyShieldMeshRenderer == null) return;

            // Update each effect slot
            energyShieldMeshRenderer.material.SetVector("_EffectPosition0", effectPositions[0]);
            energyShieldMeshRenderer.material.SetVector("_EffectPosition1", effectPositions[1]);
            energyShieldMeshRenderer.material.SetVector("_EffectPosition2", effectPositions[2]);
            energyShieldMeshRenderer.material.SetVector("_EffectPosition3", effectPositions[3]);
            energyShieldMeshRenderer.material.SetVector("_EffectPosition4", effectPositions[4]);
            energyShieldMeshRenderer.material.SetVector("_EffectPosition5", effectPositions[5]);
            energyShieldMeshRenderer.material.SetVector("_EffectPosition6", effectPositions[6]);
            energyShieldMeshRenderer.material.SetVector("_EffectPosition7", effectPositions[7]);
            energyShieldMeshRenderer.material.SetVector("_EffectPosition8", effectPositions[8]);
            energyShieldMeshRenderer.material.SetVector("_EffectPosition9", effectPositions[9]);

            // For each of the effect slots, lerp the effect strength to zero (fade out)
            for (int i = 0; i < numEffectSlots; ++i)
            {
                Vector4 temp = effectPositions[i];
                temp.w = Mathf.Lerp(temp.w, 0f, effectFadeSpeed * Time.deltaTime);
                effectPositions[i] = temp;
                
            }
        }

        // Called every frame
        protected virtual void Update()
        {
            // Update the shield effects frame each frame
            UpdateEffects();
        }
    }
}