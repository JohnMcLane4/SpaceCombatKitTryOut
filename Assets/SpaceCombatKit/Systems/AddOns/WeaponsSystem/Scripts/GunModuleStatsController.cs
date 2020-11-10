using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class GunModuleStatsController : ModuleStatsController
    {
        [SerializeField]
        protected UIFillBarController speedBar;

        [SerializeField]
        protected List<HealthFillBarController> damageStatsBars = new List<HealthFillBarController>();

        protected float maxBulletSpeed;
        protected float[] maxDamageByHealthType;


        protected void Awake()
        {
            // Fill out the max health arrays
            maxDamageByHealthType = new float[damageStatsBars.Count];
        }

        public override void UpdateMaxStats(List<Module> modulePrefabs)
        {

            // Reset gun max stats
            maxBulletSpeed = 0;
            for (int i = 0; i < maxDamageByHealthType.Length; ++i)
            {
                maxDamageByHealthType[i] = 0;
            }

            // Update gun max stats
            for (int i = 0; i < modulePrefabs.Count; ++i)
            {

                GunWeapon gunWeapon = modulePrefabs[i].GetComponent<GunWeapon>();

                if (gunWeapon != null)
                {
                    // Update the gun bullet speed
                    maxBulletSpeed = Mathf.Max(gunWeapon.GetSpeed(), maxBulletSpeed);

                    // Updeate max damage values for each health type
                    for (int j = 0; j < damageStatsBars.Count; ++j)
                    {
                        maxDamageByHealthType[j] = Mathf.Max(maxDamageByHealthType[j], gunWeapon.GetDamage(damageStatsBars[j].HealthType));
                    }
                }
            }
        }

        public override void HideStats()
        {
            base.HideStats();

            speedBar.gameObject.SetActive(false);

            for(int i = 0; i < damageStatsBars.Count; ++i)
            {
                damageStatsBars[i].gameObject.SetActive(false);
            }
        }

        public override void ShowModuleStats(Module module)
        {
            base.ShowModuleStats(module);

            GunWeapon gunWeapon = module.GetComponent<GunWeapon>();
            if (gunWeapon != null)
            {

                speedBar.gameObject.SetActive(true);
                speedBar.SetFillAmount(gunWeapon.GetSpeed() / maxBulletSpeed);

                // Update gun stats
                for (int i = 0; i < damageStatsBars.Count; ++i)
                {
                    damageStatsBars[i].gameObject.SetActive(true);
                    damageStatsBars[i].SetFillAmount(gunWeapon.GetDamage(damageStatsBars[i].HealthType) / maxDamageByHealthType[i]);
                }
            }
        }
    }
}

