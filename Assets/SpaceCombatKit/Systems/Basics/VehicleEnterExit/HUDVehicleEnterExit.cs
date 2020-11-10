using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VSX.UniversalVehicleCombat
{
    public class HUDVehicleEnterExit : MonoBehaviour
    {

        [SerializeField]
        protected VehicleEnterExitManager vehicleEnterExitManager;

        [Tooltip("The prompt that appears when the occupant can enter or exit the vehicle.")]
        [SerializeField]
        protected Text promptText;

        [Tooltip("The default message for the prompt that appears when the occupant can enter the vehicle.")]
        [SerializeField]
        protected string enterPrompt;

        [Tooltip("The default message for the prompt that appears when the occupant can exit the vehicle.")]
        [SerializeField]
        protected string exitPrompt;


        public void SetPrompts(string enterPrompt, string exitPrompt)
        {
            this.enterPrompt = enterPrompt;
            this.exitPrompt = exitPrompt;
        }


        void Update()
        {
            if (vehicleEnterExitManager.EnterableVehicles.Count > 0)
            {
                promptText.text = enterPrompt;
            }
            else if (vehicleEnterExitManager.CanExitToChild())
            {
                promptText.text = exitPrompt;
            }
            else
            {
                promptText.text = "";
            }
        }
    }
}