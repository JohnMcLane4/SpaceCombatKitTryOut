using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VSX.CameraSystem
{
    [System.Serializable]
    public class OnCameraTargetChangedEventHandler : UnityEvent<CameraTarget> { }

    [System.Serializable]
    public class OnCameraViewTargetChangedEventHandler : UnityEvent<CameraViewTarget> { }

    public class CameraEntity : MonoBehaviour
    {

        [Header("General")]

        [Tooltip("The camera target that this camera will follow when the scene starts.")]
        [SerializeField]
        protected CameraTarget startingCameraTarget;
        
        protected CameraTarget cameraTarget;
        public CameraTarget CameraTarget { get { return cameraTarget; } }

        [Tooltip("Reference to the main camera.")]
        [SerializeField]
        protected Camera mainCamera;
        public Camera MainCamera { get { return mainCamera; } }

        [Tooltip("A list of all the secondary cameras which must conform to this camera's state.")]
        [SerializeField]
        protected List<SecondaryCamera> secondaryCameras = new List<SecondaryCamera>();
        public List<SecondaryCamera> SecondaryCameras { get { return secondaryCameras; } }

        [SerializeField]
        protected float defaultFieldOfView;
        public float DefaultFieldOfView 
        { 
            get { return defaultFieldOfView; }
            set { defaultFieldOfView = value; }
        }
        
        // List of all the camera controllers in the hierarchy
        protected List<CameraController> cameraControllers = new List<CameraController>();

        // Flag for whether the camera is passive (controller disabled)
        protected bool controllersEnabled = false;
        public virtual bool ControllersEnabled
        {
            get { return controllersEnabled; }
            set
            {
                for (int i = 0; i < cameraControllers.Count; ++i)
                {
                    cameraControllers[i].SetControllerEnabled(value);
                }
            }
        }

        protected CameraViewTarget currentViewTarget;
        public CameraViewTarget CurrentViewTarget { get { return currentViewTarget; } }

        protected bool hasCameraViewTarget;
        public bool HasCameraViewTarget { get { return hasCameraViewTarget; } }

        public CameraView CurrentView { get { return hasCameraViewTarget ? currentViewTarget.CameraView : null; } }

        [Header("Events")]
        
        public OnCameraTargetChangedEventHandler onCameraTargetChanged;

        public OnCameraViewTargetChangedEventHandler onCameraViewTargetChanged;



        // Called when the component is first added to a gameobject or the component is reset
        protected virtual void Reset()
        {
            mainCamera = Camera.main;
            if(mainCamera != null)
            {
                defaultFieldOfView = Camera.main.fieldOfView;
            }
        }


        protected virtual void Awake()
		{
            // Get all the camera controllers in the hierarchy
            cameraControllers = new List<CameraController>(transform.GetComponentsInChildren<CameraController>());            
            if (cameraControllers.Count == 0)
            {
                Debug.LogWarning("No camera controllers found in this camera's hierarchy, it will do nothing. Please add one or more controllers to enable camera behaviour.");
            }

            foreach(CameraController cameraController in cameraControllers)
            {
                cameraController.SetCamera(this);
            }

            foreach (SecondaryCamera secondaryCamera in secondaryCameras)
            {
                secondaryCamera.CameraEntity = this;
            }
        }


        // Called at the start
        protected virtual void Start()
        {
            // Start targeting the starting camera target
            if (startingCameraTarget != null)
            {
                SetCameraTarget(startingCameraTarget);
            }
        }

        /// <summary>
        /// Set a new camera target to follow.
        /// </summary>
        /// <param name="target">The new camera target.</param>
        public virtual void SetCameraTarget (CameraTarget target)
		{

            if (target == cameraTarget) return;

            // Clear parent
            transform.SetParent(null);
            
            // Deactivate all the camera controllers
            for (int i = 0; i < cameraControllers.Count; ++i)
            {
                cameraControllers[i].SetControllerEnabled(false);
            }

            // Set the following camera to null on previous target
            if (cameraTarget != null)
            {
                cameraTarget.SetCamera(null);
            }

            // Update the camera target reference
            cameraTarget = null;
            if (target != null)
            {
                cameraTarget = target;
                
            }
            
            // Activate the appropriate controller(s)
            if (cameraTarget != null)
            {

                cameraTarget.SetCamera(this);

                // If no camera view targets on camera target, issue a warning
                if (cameraTarget.CameraViewTargets.Count == 0)
                {
                    Debug.LogWarning("No Camera View Target components found in camera target object's hierarchy, please add one or more.");
                }

                // Activate the appropriate camera controller(s)
                int numControllers = 0;
                for (int i = 0; i < cameraControllers.Count; ++i)
                {
                    cameraControllers[i].OnCameraTargetChanged(cameraTarget, true);
                    if (cameraControllers[i].Initialized)
                    {
                        numControllers++;
                    }
                }
                if (numControllers == 0)
                {
                    Debug.LogWarning("No compatible camera controllers found for target object, please add a compatible camera controller to this camera's hierarchy.");
                }

                onCameraTargetChanged.Invoke(cameraTarget);
            }
		}

        /// <summary>
        /// Cycle the camera view forward or backward.
        /// </summary>
        /// <param name="forward">Whether to cycle forward.</param>
        public virtual void CycleCameraView(bool forward)
        {

            // If the camera target has no camera view targets, return.
            if (cameraTarget == null || cameraTarget.CameraViewTargets.Count == 0) return;

            // Get the index of the current camera view target
            int index = cameraTarget.CameraViewTargets.IndexOf(currentViewTarget);
            index += forward ? 1 : -1;

            // Wrap the index between 0 and the number of camera view targets on the camera target.
            if (index >= cameraTarget.CameraViewTargets.Count)
            {
                index = 0;
            }
            else if (index < 0)
            {
                index = cameraTarget.CameraViewTargets.Count - 1;
            }

            // Set the new camera view target
            SetCameraViewTarget(cameraTarget.CameraViewTargets[index]);

        }

        // Set the camera view target that this camera is following
        protected virtual void SetCameraViewTarget(CameraViewTarget cameraViewTarget)
        {

            if (cameraViewTarget == null) return;

            // Update the current view target info
            this.currentViewTarget = cameraViewTarget;

            // Update the flag
            hasCameraViewTarget = this.currentViewTarget != null;

            if (cameraViewTarget.ParentCameraOnSelected)
            {
                transform.SetParent(cameraViewTarget.transform);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                transform.SetParent(null);
            }

            cameraViewTarget.OnSelected();

            onCameraViewTargetChanged.Invoke(cameraViewTarget);

        }

        /// <summary>
        /// Select a new camera view.
        /// </summary>
        /// <param name="newView">The new camera view.</param>
        public virtual void SetView(CameraView newView)
		{

            // If no camera target or null view, set to null and exit.
            if (newView == null || cameraTarget == null)
            {
                SetCameraViewTarget(null);
                return;
            }

            // Search all camera views on camera target for desired view
            for (int i = 0; i < cameraTarget.CameraViewTargets.Count; ++i)
			{
				if (cameraTarget.CameraViewTargets[i].CameraView == newView)
				{
                    SetCameraViewTarget(cameraTarget.CameraViewTargets[i]);
                    return;
				}
			}

            // If none found, default to the first available
            if (cameraTarget.CameraViewTargets.Count > 0)
            {
                // Set the first available Camera View Target
                SetCameraViewTarget(cameraTarget.CameraViewTargets[0]);

                if (newView != null)
                {
                    // Issue a warning
                    Debug.LogWarning("No CameraViewTarget found for Camera View type " + newView.ToString() + ". Defaulting to " +
                        currentViewTarget.CameraView.ToString());
                }
            }
            else
            {
                SetView(null);

                // Issue a warning
                Debug.LogWarning("No Camera View Target found on the camera target object, camera will not work. Please add one or more CameraViewTarget components to the camera target object's hierarchy.");
            }	
		}

        /// <summary>
        /// Set the field of view for the camera.
        /// </summary>
        /// <param name="newFieldOfView">The new field of view.</param>
        public virtual void SetFieldOfView(float newFieldOfView)
        {
            mainCamera.fieldOfView = newFieldOfView;
        }

        protected virtual void Update()
        {
            for (int i = 0; i < secondaryCameras.Count; ++i)
            {
                secondaryCameras[i].OnFieldOfViewChanged(mainCamera.fieldOfView);
            }
        }
    }
}
