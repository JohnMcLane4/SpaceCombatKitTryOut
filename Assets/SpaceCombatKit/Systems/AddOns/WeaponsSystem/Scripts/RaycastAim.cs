using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.CameraSystem;

namespace VSX.UniversalVehicleCombat
{
    public class RaycastAim : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField]
        protected Weapons weapons;

        [SerializeField]
        protected bool ignoreTriggerColliders = false;

        [SerializeField]
        protected LayerMask aimMask;

        [SerializeField]
        protected Transform aimRaycastOrigin;

        [Header("Camera Based Aiming")]

        [SerializeField]
        protected bool useCameraForRaycast = true;

        [SerializeField]
        protected CameraTarget cameraTarget;

        protected Vector3 aimPosition;
        public Vector3 AimPosition { get { return aimPosition; } }


        protected virtual void Reset()
        {
            weapons = GetComponent<Weapons>();
            cameraTarget = GetComponent<CameraTarget>();

            aimMask = ~0;
        }

        protected virtual void Awake()
        {
            if (cameraTarget != null)
            {
                cameraTarget.onCameraTargeting.AddListener(OnCameraFollowing);
            }
        }

        public virtual void OnCameraFollowing(Camera cam)
        {
            if (useCameraForRaycast) aimRaycastOrigin = cam.transform;
        }

        public virtual void Aim()
        {
            if (aimRaycastOrigin == null)
            {
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(aimRaycastOrigin.position, aimRaycastOrigin.forward, out hit, 1000, aimMask, ignoreTriggerColliders ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide))
            {
                aimPosition = hit.point;
                for (int i = 0; i < weapons.AimAssists.Count; ++i)
                {
                    weapons.AimAssists[i].Aim(aimPosition);
                }
            }
            else
            {
                aimPosition = aimRaycastOrigin.position + aimRaycastOrigin.forward * 1000;
                for (int i = 0; i < weapons.AimAssists.Count; ++i)
                {
                    weapons.AimAssists[i].Aim(aimPosition);
                }
            }
        }

        protected virtual void Update()
        {
            Aim();
        }
    }
}
