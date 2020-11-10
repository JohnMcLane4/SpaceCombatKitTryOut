using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VSX.UniversalVehicleCombat
{
    public class LoadoutStats : MonoBehaviour
    {

        [Header("Ships")]

        [SerializeField]
        protected Text shipLabel;

        [SerializeField]
        protected Text shipDescription;

        protected float maxShipSpeed;
        protected float maxShipAgility;
        protected float[] maxShipHealthByHealthType;

        [SerializeField]
        protected UIFillBarController shipSpeedStatBar;

        [SerializeField]
        protected UIFillBarController shipAgilityStatBar;

        [SerializeField]
        protected List<HealthFillBarController> shipHealthStatsBars = new List<HealthFillBarController>();

        [Header("Modules")]

        public List<ModuleStatsController> moduleStatsControllers = new List<ModuleStatsController>();



        protected void Awake()
        {
            // Fill out the max health arrays
            maxShipHealthByHealthType = new float[shipHealthStatsBars.Count];
        }


        protected float GetAgility (VehicleEngines3D engines)
        {
            return ((engines.MaxSteeringForces.x + engines.MaxSteeringForces.y + engines.MaxSteeringForces.z) / 3f);
        }

        protected float GetSpeed(VehicleEngines3D engines)
        {
            return (engines.GetDefaultMaxSpeedByAxis(false).z);
        }


        public void UpdateMaxStats (List<Module> modulePrefabs)
        {
            for (int i = 0; i < moduleStatsControllers.Count; ++i)
            {
                moduleStatsControllers[i].UpdateMaxStats(modulePrefabs);
            }
        }

        
        protected void UpdateShipStats(List<Vehicle> vehicles)
        {

            // Reset ship max stats
            maxShipSpeed = 0;
            maxShipAgility = 0;

            for (int i = 0; i < maxShipHealthByHealthType.Length; ++i)
            {
                maxShipHealthByHealthType[i] = 0;
            }

            // Update ship max stats
            for (int i = 0; i < vehicles.Count; ++i)
            {
                // Get the engines component
                VehicleEngines3D engines = vehicles[i].GetComponentInChildren<VehicleEngines3D>();
                if (engines == null)
                {
                    Debug.LogError("Attempting to calculate ship performance metrics for a vehicle which does not contain a VehicleEngines3D component");
                }

                // Update max ship speed
                maxShipSpeed = Mathf.Max(maxShipSpeed, GetSpeed(engines));

                // Update max ship agility
                maxShipAgility = Mathf.Max(maxShipAgility, GetAgility(engines));

                // Get the ships's health component
                Health health = vehicles[i].GetComponent<Health>();
                if (health != null)
                {
                    // Update max ship health for each health type
                    for (int j = 0; j < shipHealthStatsBars.Count; ++j)
                    {
                        maxShipHealthByHealthType[j] = Mathf.Max(maxShipHealthByHealthType[j], health.GetMaxHealthByType(shipHealthStatsBars[j].HealthType));
                    }
                }
            }
        }

        public void ShowStats(Module module)
        {
            for (int i = 0; i < moduleStatsControllers.Count; ++i)
            {
                moduleStatsControllers[i].HideStats();
            }

            for (int i = 0; i < moduleStatsControllers.Count; ++i)
            {
                moduleStatsControllers[i].ShowModuleStats(module);
            }
        }


        public void ShowStats(Vehicle vehicle)
        {
            shipLabel.text = vehicle.Label;

            // Show engine stats
            VehicleEngines3D engines = vehicle.GetComponentInChildren<VehicleEngines3D>();
            if (engines == null)
            {
                shipSpeedStatBar.SetFillAmount(GetSpeed(engines) / maxShipSpeed);

                shipAgilityStatBar.SetFillAmount(GetAgility(engines) / maxShipAgility);
            }

            // Show health stats
            Health health = vehicle.GetComponent<Health>();
            if (health != null)
            {
                for (int i = 0; i < shipHealthStatsBars.Count; ++i)
                {
                    shipHealthStatsBars[i].SetFillAmount(health.GetMaxHealthByType(shipHealthStatsBars[i].HealthType) /
                                                        maxShipHealthByHealthType[i]);
                }
            }
        }
    }
}