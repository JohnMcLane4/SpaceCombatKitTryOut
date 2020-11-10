using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace VSX.UniversalVehicleCombat
{
    public class PlayerVehicleEnterExitInput : VehicleInput
    {

        [Header("Enter/Exit Settings")]

        [SerializeField]
        protected GameAgent gameAgent;

        // The dependencies on the current vehicle
        protected VehicleEnterExitManager vehicleEnterExitManager;
        protected HUDVehicleEnterExit hudController;

        [SerializeField]
        protected bool setEnterExitPrompts = true;
        
        [SerializeField]
        protected CustomInput enterExitInput = new CustomInput("General Vehicle Controls", "Enter/Exit Vehicle", KeyCode.F);


        /// <summary>
        /// Initialize the vehicle input component.
        /// </summary>
        /// <param name="vehicle">The vehicle to intialize for.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {          
            // Update the dependencies
            vehicleEnterExitManager = vehicle.GetComponent<VehicleEnterExitManager>();
            if (vehicleEnterExitManager == null)
            {
                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize - the required " + vehicleEnterExitManager.GetType().Name + " component was not found on the vehicle.");
                }
                return false;
            }

            hudController = vehicle.GetComponentInChildren<HUDVehicleEnterExit>();
            if (debugInitialization)
            {
                Debug.Log(GetType().Name + " successfully initialized.");
            }

            return true;

        }

        // Called every frame
        protected override void InputUpdate()
        {
            if (hudController != null && setEnterExitPrompts)
            {
                hudController.SetPrompts("PRESS " + enterExitInput.GetInputAsString() + " TO ENTER", 
                                            "PRESS " + enterExitInput.GetInputAsString() + " TO EXIT"); 
            }

            if (vehicleEnterExitManager.EnterableVehicles.Count > 0)
            {
                // Check for input
                if (enterExitInput.Down())
                {
                    Vehicle parent = vehicleEnterExitManager.EnterableVehicles[0].Vehicle;
                    vehicleEnterExitManager.EnterParent(0);
                    gameAgent.EnterVehicle(parent);
                }
            }
            else if (vehicleEnterExitManager.CanExitToChild())
            {
                // Check for input
                if (enterExitInput.Down())
                {
                    Vehicle child = vehicleEnterExitManager.Child.Vehicle;
                    vehicleEnterExitManager.ExitToChild();
                    gameAgent.EnterVehicle(child);
                }
            }
        }
    }
}
