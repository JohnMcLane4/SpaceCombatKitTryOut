using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class PoweredProjectileLauncher : ProjectileLauncher, IPowerConsumer
    {
        [Header("Power")]

        public bool usePower = true;

        protected Power power;
        public Power Power { set { power = value; } }

        public float powerDrawPerLaunch;

        public override void LaunchProjectile()
        {
            if (usePower && power == null) return;

            if (usePower)
            {
                if (power.HasStoredPower(PoweredSubsystemType.Weapons, powerDrawPerLaunch))
                {
                    power.DrawStoredPower(PoweredSubsystemType.Weapons, powerDrawPerLaunch);
                    base.LaunchProjectile();
                }
            }
            else
            {
                base.LaunchProjectile();
            }
        }
    }
}
