using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Represents a gimbal that is used to target weapons fire by using raycast detection to adjust weapons fire.
    /// </summary>
    public class TargetingGimbal : GimbalController
    {

        [Header("Targeting Gimbal")]

        [Tooltip("Whether to unparent the gimbal and set it to follow position of a transform. Prevents view from changing when a vehicle is rotating.")]
        [SerializeField]
        protected bool unparentAndFollowPosition = true;

        [Tooltip("The target transform to follow the position of.")]
        [SerializeField]
        protected Transform followTarget;

        [Tooltip("The view gimbal object, which must be un-parented from the vehicle so as to maintain rotation when the vehicle is turning.")]
        [SerializeField]
        protected Transform viewGimbalObject;

        [Header("Aim Distance Adjustment")]

        [Tooltip("A transform that represents the point in space where turret fire should converge.")]
        [SerializeField]
        protected Transform aimTarget;

        [Tooltip("The origin of the raycast for adjusting aim distance.")]
        [SerializeField]
        protected Transform aimRaycastOrigin;

        [Tooltip("The raycast distance for calculating where the turret aim will converge when aiming at targets at varying distances.")]
        [SerializeField]
        protected float aimRaycastDistance = 5000;

        [Tooltip("Whether to prevent collision with self when adjusting aim distance.")]
        [SerializeField]
        protected bool preventSelfCollision = true;

        [Tooltip("The root transform of the firer.")]
        [SerializeField]
        protected Transform selfRootTransform;


        protected void Reset()
        {
            selfRootTransform = transform;
            followTarget = transform;
        }

        private void Start()
        {
            if (unparentAndFollowPosition && viewGimbalObject != null) viewGimbalObject.SetParent(null);
        }

        private void Update()
        {
            // Raycast
            RaycastHit[] hits;
            hits = Physics.RaycastAll(aimRaycastOrigin.position, aimRaycastOrigin.forward, aimRaycastDistance);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));    // Sort by distance

            bool hitFound = false;
            for (int i = 0; i < hits.Length; ++i)
            {

                if (!hits[i].collider.isTrigger)
                {
                    // Prevent aiming at point on self
                    if (preventSelfCollision)
                    {
                        DamageReceiver damageReceiver = hits[i].collider.GetComponent<DamageReceiver>();
                        if (damageReceiver != null)
                        {
                            if (damageReceiver.RootTransform == selfRootTransform)
                            {
                                continue;
                            }
                        }
                    }

                    aimTarget.position = hits[i].point;
                    aimTarget.localPosition = new Vector3(0, 0, aimTarget.transform.localPosition.z);
                    hitFound = true;
                    break;
                }
            }

            if (!hitFound)
            {
                aimTarget.localPosition = new Vector3(0, 0, aimRaycastDistance);
            }
        }

        
        protected void LateUpdate()
        {
            if (unparentAndFollowPosition && viewGimbalObject != null)
            {
                viewGimbalObject.position = followTarget.position;
                viewGimbalObject.rotation = Quaternion.identity;
            }
        }
    }
}

