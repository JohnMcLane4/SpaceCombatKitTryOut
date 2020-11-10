using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Unity event to run functions when beam state changes.
    /// </summary>
    [System.Serializable]
    public class OnBeamStateChangedEventHandler : UnityEvent<BeamState> { };

    /// <summary>
    /// Unity event to run functions when beam level changes.
    /// </summary>
    [System.Serializable]
    public class OnBeamLevelSetEventHandler : UnityEvent<float> { };

    /// <summary>
    /// Unity event to run functions when a beam hits a collider.
    /// </summary>
    [System.Serializable]
    public class OnBeamControllerHitDetectedEventHandler : UnityEvent<RaycastHit> { };

    /// <summary>
    /// Unity event to run functions when a beam stops hitting a collider
    /// </summary>
    [System.Serializable]
    public class OnBeamControllerHitNotDetectedEventHandler : UnityEvent { };

    

    /// <summary>
    /// Controls a beam for e.g. a weapon.
    /// </summary>
    public class BeamController : MonoBehaviour, IRootTransformUser
    {

        [Header("Beam Parameters")]

        [SerializeField]
        protected LayerMask hitMask = Physics.DefaultRaycastLayers;

        [Tooltip("Whether to ignore trigger colliders.")]
        [SerializeField]
        protected bool ignoreTriggerColliders = false;

        [Tooltip("Whether to ignore collision with the object or vehicle that this object came from.")]
        [SerializeField]
        protected bool ignoreHierarchyCollision = true;

        [SerializeField]
        protected LineRenderer beamLineRenderer;

        [SerializeField]
        protected BeamHitEffectController beamHitEffectController;

        [SerializeField]
        protected Transform beamSpawn;

        [SerializeField]
        protected float range = 1000;

        [SerializeField]
        protected float maxBeamLevel = 1;
        public float MaxBeamLevel { set { maxBeamLevel = value; } }

        [SerializeField]
        protected string beamColorShaderProperty = "_Color";

        protected BeamState currentBeamState = BeamState.Off;

        protected float beamStateStartTime = 0;

        protected float beamLevel = 0;

        protected bool firing = false;

        [Header("Continuous")]

        [SerializeField]
        protected float beamFadeInTime = 0.33f;

        [SerializeField]
        protected AnimationCurve beamFadeInCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        protected float beamFadeOutTime = 0.33f;

        [SerializeField]
        protected AnimationCurve beamFadeOutCurve = AnimationCurve.Linear(0, 1, 1, 0);

        [Header("Pulsed")]

        [SerializeField]
        protected bool isPulsed = false;

        [SerializeField]
        protected float pulseDuration = 0.75f;

        [SerializeField]
        protected AnimationCurve pulseCurve = AnimationCurve.Linear(0, 1, 1, 1);

        [Header("Beam Controller Events")]

        public UnityEvent onBeamStarted;

        public UnityEvent onBeamStopped;

        public UnityEvent OnBeamActive;

        // Beam level changed unity event
        public OnBeamLevelSetEventHandler onBeamLevelSet;

        // Beam hit detected unity event
        public OnBeamControllerHitDetectedEventHandler onHitDetected;

        // Beam hit not detected unity event
        public OnBeamControllerHitNotDetectedEventHandler onHitNotDetected;

        protected Transform rootTransform;
        public Transform RootTransform
        {
            set { rootTransform = value; }
        }


        // Called when scene starts
        protected virtual void Start()
        {

            if (rootTransform == null) rootTransform = transform.root;

            SetBeamLevel(0);
            if (beamHitEffectController != null)
            {
                beamHitEffectController.SetActivation(false);
            }
        }

        // Set the beam state
        protected virtual void SetBeamState(BeamState newBeamState)
        {

            switch (newBeamState)
            {
                case BeamState.FadingIn:

                    currentBeamState = BeamState.FadingIn;
                    beamStateStartTime = Time.time - beamLevel * beamFadeInTime;    // Assume linear fade in/out
                    onBeamStarted.Invoke();
                    break;

                case BeamState.FadingOut:

                    currentBeamState = BeamState.FadingOut;
                    beamStateStartTime = Time.time - (1 - beamLevel) * beamFadeOutTime;     // Assume linear fade in/out
                    break;

                case BeamState.Sustaining:

                    currentBeamState = BeamState.Sustaining;
                    beamStateStartTime = Time.time;
                    break;

                case BeamState.Off:

                    currentBeamState = BeamState.Off;
                    beamStateStartTime = Time.time;
                    onBeamStopped.Invoke();
                    break;

                case BeamState.Pulsing:

                    currentBeamState = BeamState.Pulsing;
                    beamStateStartTime = Time.time;
                    onBeamStarted.Invoke();
                    break;

            }
        }


        // Do a hit scan
        protected virtual bool DoHitScan()
        {

            // Raycast
            RaycastHit[] hits;
            hits = Physics.RaycastAll(beamSpawn.position, beamSpawn.forward, range, hitMask, ignoreTriggerColliders ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));    // Sort by distance

            for (int i = 0; i < hits.Length; ++i)
            {

                DamageReceiver damageReceiver = hits[i].collider.GetComponent<DamageReceiver>();
                if (ignoreHierarchyCollision && rootTransform != null && damageReceiver != null && damageReceiver.RootTransform == rootTransform)
                {
                    continue;
                }
                
                UpdateBeamPositions(beamSpawn.position, hits[i].point);

                OnHitDetected(hits[i]);

                return true;

            }

            UpdateBeamPositions(beamSpawn.position, beamSpawn.position + beamSpawn.forward * range);

            OnHitNotDetected();

            return false;
        }


        protected virtual void OnHitDetected(RaycastHit hit)
        {

            // Update hit effect
            if (beamHitEffectController != null)
            {
                beamHitEffectController.SetActivation(true);
                beamHitEffectController.OnHit(hit);
            }

            onHitDetected.Invoke(hit);
        }


        protected virtual void OnHitNotDetected()
        {

            // Disable hit effect
            if (beamHitEffectController != null) beamHitEffectController.SetActivation(false);

            onHitNotDetected.Invoke();
        }


        protected virtual void UpdateBeamPositions(Vector3 start, Vector3 end)
        {
            beamLineRenderer.SetPosition(0, beamLineRenderer.transform.InverseTransformPoint(start));
            beamLineRenderer.SetPosition(1, beamLineRenderer.transform.InverseTransformPoint(end));
        }


        /// <summary>
        /// Set the beam level.
        /// </summary>
        /// <param name="level">Beam level.</param>
        public virtual void SetBeamLevel(float level)
        {

            beamLevel = Mathf.Clamp(level, 0, maxBeamLevel);
            
            // Set the color
            if (beamLineRenderer.material.HasProperty(beamColorShaderProperty))
            {
                Color c = beamLineRenderer.material.GetColor(beamColorShaderProperty);
                c.a = beamLevel;
                beamLineRenderer.material.SetColor(beamColorShaderProperty, c);
            }            

            // Update hit effect
            if (beamHitEffectController != null)
            {
                beamHitEffectController.SetLevel(beamLevel);
            }
            
            // Call event
            onBeamLevelSet.Invoke(beamLevel);
        }


        /// <summary>
        /// Start triggering the beam.
        /// </summary>
        public virtual void StartTriggering()
        {
            if (!firing)
            {
                if (isPulsed)
                {
                    if (currentBeamState == BeamState.Off)
                    {
                        SetBeamState(BeamState.Pulsing);
                    }
                }
                else
                {
                    SetBeamState(BeamState.FadingIn);
                }
            }

            firing = true;
        }


        /// <summary>
        /// Stop triggering the beam.
        /// </summary>
        public virtual void StopTriggering()
        {
            if (firing)
            {
                if (!isPulsed)
                {
                    SetBeamState(BeamState.FadingOut);
                }
            }

            firing = false;
        }

        public virtual void TriggerOnce()
        {
            if (!firing)
            {
                if (isPulsed)
                {
                    SetBeamState(BeamState.Pulsing);
                }
            }
        }


        protected virtual void LateUpdate()
        {
            
            // Handle beam transitions
            switch (currentBeamState)
            {
                case BeamState.FadingIn:

                    float fadeInAmount = (Time.time - beamStateStartTime) / beamFadeInTime;
                    if (fadeInAmount > 1)
                    {
                        SetBeamLevel(1);
                        SetBeamState(BeamState.Sustaining);
                    }
                    else
                    {
                        SetBeamLevel(Mathf.Clamp(fadeInAmount, 0, 1));
                    }
                    break;

                case BeamState.FadingOut:

                    float fadeOutAmount = (Time.time - beamStateStartTime) / beamFadeOutTime;
                    if (fadeOutAmount > 1)
                    {
                        SetBeamLevel(0);
                        SetBeamState(BeamState.Off);
                        if (beamHitEffectController != null) beamHitEffectController.SetActivation(false);
                    }
                    else
                    {
                        SetBeamLevel(Mathf.Clamp(1 - fadeOutAmount, 0, 1));
                    }
                    break;

                case BeamState.Sustaining:

                    SetBeamLevel(1);
                    
                    break;

                case BeamState.Off:
                    SetBeamLevel(0);
                    break;

                case BeamState.Pulsing:

                    float pulseAmount = (Time.time - beamStateStartTime) / pulseDuration;
                    if (pulseAmount > 1)
                    {
                        SetBeamState(BeamState.Off);
                        SetBeamLevel(0);
                    }
                    else
                    {
                        SetBeamLevel(pulseCurve.Evaluate(pulseAmount));
                    }
                    break;
            }

            if (currentBeamState != BeamState.Off)
            {
                DoHitScan();
                OnBeamActive.Invoke();
            }
        }
    }
}