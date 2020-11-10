using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Input script for controlling the steering and movement of a space fighter vehicle.
    /// </summary>
    public class PlayerSpaceVehicleFlightControls : VehicleInput
    {
        [Header("Control Scheme")]

        [Tooltip("Whether the vehicle should yaw when rolling.")]
        [SerializeField]
        protected bool linkYawAndRoll = false;

        [Tooltip("How much the vehicle should yaw when rolling.")]
        [SerializeField]
        protected float yawRollRatio = 1;

        [Header("Mouse Steering")]

        [Tooltip("Whether the mouse position should control the steering.")]
        [SerializeField]
        protected bool mouseSteeringEnabled = true;

        [Tooltip("How far the mouse reticule is allowed to get from the screen center.")]
        [SerializeField]
        protected float maxReticuleDistanceFromCenter = 0.4f;

        [SerializeField]
        protected float reticuleMovementSpeed = 30f;

        [SerializeField]
        protected string mouseDeltaXAxisName = "Mouse X";

        [SerializeField]
        protected string mouseDeltaYAxisName = "Mouse Y";

        [Tooltip("Invert the pitch (local X axis rotation) input.")]
        [SerializeField]
        protected bool mouseVerticalInverted = false;

        [Tooltip("How much the ship pitches (local X axis rotation) based on the mouse distance from screen center.")]
        [SerializeField]
        protected float mousePitchSensitivity = 1;

        [Tooltip("How much the ship yaws (local Y axis rotation) based on the mouse distance from screen center.")]
        [SerializeField]
        protected float mouseYawSensitivity = 1;

        [Tooltip("How much the ship rolls (local Z axis rotation) based on the mouse distance from screen center.")]
        [SerializeField]
        protected float mouseRollSensitivity = 1;

        [Tooltip("The fraction of the viewport (based on the screen width) around the screen center inside which the mouse position does not affect the ship steering.")]
        [SerializeField]
        protected float mouseDeadRadius = 0;
        

        [Header("Keyboard Steering")]

        [Tooltip("Invert the pitch (local X rotation) input.")]
        [SerializeField]
        protected bool keyboardVerticalInverted = false;

        [Tooltip("Whether keyboard steering is enabled.")]
        [SerializeField]
        protected bool keyboardSteeringEnabled = false;

        [Tooltip("The custom input controls for the pitch (local X axis rotation) steering.")]
        [SerializeField]
        protected CustomInput pitchAxisInput = new CustomInput("Flight Controls", "Pitch", "Vertical");

        [Tooltip("The custom input controls for the yaw (local Y axis rotation) steering.")]
        [SerializeField]
        protected CustomInput yawAxisInput = new CustomInput("Flight Controls", "Yaw", "Horizontal");

        [Tooltip("The custom input controls for the roll (local Z axis rotation) steering.")]
        [SerializeField]
        protected CustomInput rollAxisInput = new CustomInput("Flight Controls", "Roll", "Roll");


        [Header("Throttle")]

        [Tooltip("The custom input controls for increasing the throttle.")]
        [SerializeField]
        protected CustomInput throttleUpInput = new CustomInput("Flight Controls", "Throttle Up", KeyCode.Z);

        [Tooltip("The custom input controls for decreasing the throttle.")]
        [SerializeField]
        protected CustomInput throttleDownInput = new CustomInput("Flight Controls", "Throttle Down", KeyCode.X);

        [Tooltip("How fast the throttle increases/decreases for each input axis.")]
        [SerializeField]
        protected float throttleSensitivity = 1;

        [Tooltip("Whether to set the throttle value using the Throttle Axis Input value.")]
        [SerializeField]
        protected bool setThrottle = false;

        [Tooltip("The custom input axis for controlling the throttle.")]
        [SerializeField]
        protected CustomInput throttleAxisInput = new CustomInput("Flight Controls", "Move Forward/Backward", "Vertical");

        [Tooltip("The custom input axis for strafing the ship vertically.")]
        [SerializeField]
        protected CustomInput strafeVerticalInput = new CustomInput("Flight Controls", "Strafe Vertical", "Strafe Vertical");

        [Tooltip("The custom input axis for strafing the ship horizontally.")]
        [SerializeField]
        protected CustomInput strafeHorizontalInput = new CustomInput("Flight Controls", "Strafe Horizontal", "Strafe Horizontal");

        // The rotation, translation and boost inputs that are updated each frame
        protected Vector3 steeringInputs = Vector3.zero;
        protected Vector3 movementInputs = Vector3.zero;
        protected Vector3 boostInputs = Vector3.zero;

        protected bool steeringEnabled = true;

        protected bool movementEnabled = true;

        [Header("Boost")]

        [SerializeField]
        protected CustomInput boostInput = new CustomInput("Flight Controls", "Boost", KeyCode.Tab);

        // Reference to the engines component on the current vehicle
        protected VehicleEngines3D spaceVehicleEngines;

        protected HUDCursor hudCursor;
        protected Vector3 reticuleViewportPosition = new Vector3(0.5f, 0.5f, 0);



        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The new vehicle.</param>
        /// <returns>Whether the input script succeeded in initializing.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {

            if (!base.Initialize(vehicle)) return false;

            // Clear dependencies
            spaceVehicleEngines = null;
          
            // Make sure the vehicle has a space vehicle engines component
            spaceVehicleEngines = vehicle.GetComponent<VehicleEngines3D>();

            hudCursor = vehicle.GetComponentInChildren<HUDCursor>();

            if (spaceVehicleEngines == null)
            {
                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize - the required " + spaceVehicleEngines.GetType().Name + " component was not found on the vehicle.");
                }
                return false;
            }

            if (debugInitialization)
            {
                Debug.Log(GetType().Name + " component successfully initialized.");
            }

            return true;
        }


        /// <summary>
        /// Stop the input.
        /// </summary>
        public override void DisableInput()
        {

            base.DisableInput();

            // Reset the space vehicle engines to idle
            if (spaceVehicleEngines != null)
            {
                // Set steering to zero
                steeringInputs = Vector3.zero;
                spaceVehicleEngines.SetSteeringInputs(steeringInputs);

                // Set movement to zero
                movementInputs = Vector3.zero;
                spaceVehicleEngines.SetMovementInputs(movementInputs);

                // Set boost to zero
                boostInputs = Vector3.zero;
                spaceVehicleEngines.SetBoostInputs(boostInputs);
            }
        }

        /// <summary>
        /// Enable steering input.
        /// </summary>
        public virtual void EnableSteering()
        {
            steeringEnabled = true;
        }


        /// <summary>
        /// Disable steering input.
        /// </summary>
        /// <param name="clearCurrentValues">Whether to clear current steering values.</param>
        public virtual void DisableSteering(bool clearCurrentValues)
        {
            steeringEnabled = false;

            if (clearCurrentValues)
            {
                // Set steering to zero
                steeringInputs = Vector3.zero;
                spaceVehicleEngines.SetSteeringInputs(steeringInputs);
            }
        }


        /// <summary>
        /// Enable movement input.
        /// </summary>
        public virtual void EnableMovement()
        {
            movementEnabled = true;
        }

        /// <summary>
        /// Disable the movement input.
        /// </summary>
        /// <param name="clearCurrentValues">Whether to clear current throttle values.</param>
        public virtual void DisableMovement(bool clearCurrentValues)
        {
            movementEnabled = false;

            if (clearCurrentValues)
            {
                // Set movement to zero
                movementInputs = Vector3.zero;
                spaceVehicleEngines.SetMovementInputs(movementInputs);

                // Set boost to zero
                boostInputs = Vector3.zero;
                spaceVehicleEngines.SetBoostInputs(boostInputs);
            }
        }


        /// <summary>
        /// Called to toggle the mouse input in this script.
        /// </summary>
        /// <param name="setEnabled">Whether the mouse input is to be enabled.</param>
        public virtual void SetMouseInputEnabled(bool setEnabled)
        {
            mouseSteeringEnabled = setEnabled;
        }


        protected void UpdateReticulePosition(Vector3 mouseDelta)
        {
            // Add the delta 
            reticuleViewportPosition += new Vector3(mouseDelta.x / Screen.width, mouseDelta.y / Screen.height, 0); 

            // Center it
            Vector3 centeredReticuleViewportPosition = reticuleViewportPosition - new Vector3(0.5f, 0.5f, 0);

            // Prevent distortion before clamping
            centeredReticuleViewportPosition.x *= Screen.width / Screen.height;

            // Clamp
            centeredReticuleViewportPosition = Vector3.ClampMagnitude(centeredReticuleViewportPosition, maxReticuleDistanceFromCenter);

            // Convert back to proper viewport
            centeredReticuleViewportPosition.x /= Screen.width / Screen.height;

            reticuleViewportPosition = centeredReticuleViewportPosition + new Vector3(0.5f, 0.5f, 0);

        }


        // Do mouse steering
        protected virtual void MouseSteeringUpdate()
        {
            Vector3 centeredViewportPos = reticuleViewportPosition - new Vector3(0.5f, 0.5f, 0);

            centeredViewportPos.x *= (float)Screen.width / Screen.height;

            float amount = Mathf.Max(centeredViewportPos.magnitude - mouseDeadRadius, 0) / (maxReticuleDistanceFromCenter - mouseDeadRadius);

            centeredViewportPos.x /= (float)Screen.width / Screen.height;

            Vector3 inputs = amount * centeredViewportPos.normalized;

            // Update pitch
            steeringInputs.x = Mathf.Clamp((mouseVerticalInverted ? 1 : -1) * inputs.y * mousePitchSensitivity, -1f, 1f);

            // Linked yaw and roll
            if (linkYawAndRoll)
            {
                Roll(Mathf.Clamp(-inputs.x * mouseRollSensitivity, -1f, 1f));
                steeringInputs.y = Mathf.Clamp(-steeringInputs.z * yawRollRatio, -1f, 1f);
            }
            // Separate axes
            else
            {
                // Roll
                Roll(rollAxisInput.FloatValue());

                // Yaw
                steeringInputs.y = Mathf.Clamp(inputs.x * mouseYawSensitivity, -1f, 1f);
            }

            if (hudCursor != null)
            {
                // Position the reticule
                hudCursor.SetViewportPosition(reticuleViewportPosition);
            }
        }


        // Do keyboard steering
        protected virtual void KeyboardSteeringUpdate()
        {
            // Pitch
            steeringInputs.x = (keyboardVerticalInverted ? -1 : 1) * pitchAxisInput.FloatValue();

            // Linked yaw and roll
            if (linkYawAndRoll)
            {
                Roll(-yawAxisInput.FloatValue());
                steeringInputs.y = Mathf.Clamp(-steeringInputs.z * yawRollRatio, -1f, 1f);
            }
            // Separate axes
            else
            {
                // Roll
                Roll(rollAxisInput.FloatValue());

                // Yaw
                steeringInputs.y = yawAxisInput.FloatValue();
            }
        }


        // Do movement
        protected virtual void MovementUpdate()
        {
            // Forward / backward movement
            movementInputs = spaceVehicleEngines.MovementInputs;

            if (setThrottle)
            {
                movementInputs.z = throttleAxisInput.FloatValue();
            }
            else
            {
                if (throttleUpInput.Pressed())
                {
                    movementInputs.z += throttleSensitivity * Time.deltaTime;
                }
                else if (throttleDownInput.Pressed())
                {
                    movementInputs.z -= throttleSensitivity * Time.deltaTime;
                }
            }  

            // Left / right movement
            movementInputs.x = strafeHorizontalInput.FloatValue();

            // Up / down movement
            movementInputs.y = strafeVerticalInput.FloatValue();

            // Boost
            if (boostInput.Down())
            {
                SetBoost(1);
            }
            else if (boostInput.Up())
            {
                SetBoost(0);
            }
        }


        public void Roll(float rollAmount)
        {
            steeringInputs.z = Mathf.Clamp(rollAmount, -1, 1);
        }

        public void SetBoost(float boostAmount)
        {
            boostInputs = new Vector3(0f, 0f, boostAmount);
        }


        protected override void InputUpdate()
        {
            UpdateReticulePosition(reticuleMovementSpeed * new Vector3(Input.GetAxis(mouseDeltaXAxisName), Input.GetAxis(mouseDeltaYAxisName), 0));

            if (steeringEnabled)
            {
                // Do mouse or keyboard steering
                if (mouseSteeringEnabled)
                {
                    MouseSteeringUpdate();
                }
                else if (keyboardSteeringEnabled)
                {
                    KeyboardSteeringUpdate();
                }

                // Implement steering
                spaceVehicleEngines.SetSteeringInputs(steeringInputs);
            }
            
            if (movementEnabled)
            {
                // Update movement
                MovementUpdate();
                
                // Implement movement
                spaceVehicleEngines.SetMovementInputs(movementInputs);
                
                // Implement boost
                spaceVehicleEngines.SetBoostInputs(boostInputs);
                
            }
        }
    }
}