using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class DestroyAllVehicles : MonoBehaviour
    {
        public void Do()
        {
            Vehicle[] vehicles = GameObject.FindObjectsOfType<Vehicle>();
            foreach(Vehicle vehicle in vehicles)
            {
                vehicle.Destroy();
            }
        }
    }
}
