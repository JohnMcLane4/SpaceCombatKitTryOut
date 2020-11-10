using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class MissileModuleStatsController : ModuleStatsController
    {
        [SerializeField]
        protected UIFillBarController speedBar;

        [SerializeField]
        protected UIFillBarController agilityBar;

        [SerializeField]
        protected UIFillBarController rangeBar;

        [SerializeField]
        protected List<HealthFillBarController> damageStatsBars = new List<HealthFillBarController>();

        protected float maxSpeed;
        protected float maxAgility;
        protected float maxRange;
        protected float[] maxDamageByHealthType;


        protected void Awake()
        {
            // Fill out the max health arrays
            maxDamageByHealthType = new float[damageStatsBars.Count];
        }



        public override void UpdateMaxStats(List<Module> modulePrefabs)
        {
            // Reset missile max stats
            maxSpeed = 0;
            for (int i = 0; i < maxDamageByHealthType.Length; ++i)
            {
                maxDamageByHealthType[i] = 0;
            }

            // Update gun max stats
            for (int i = 0; i < modulePrefabs.Count; ++i)
            {
                MissileWeapon missileWeapon = modulePrefabs[i].GetComponent<MissileWeapon>();
                if (missileWeapon != null)
                {
                    // Update the missile speed
                    maxSpeed = Mathf.Max(missileWeapon.GetMissileSpeed(), maxSpeed);

                    // Update the missile agility
                    maxAgility = Mathf.Max(missileWeapon.GetMissileAgility(), maxAgility);

                    // Update the max missile range
                    maxRange = Mathf.Max(missileWeapon.GetMissileRange(), maxRange);

                    // Update max damage values for each health type
                    for (int j = 0; j < damageStatsBars.Count; ++j)
                    {
                        maxDamageByHealthType[j] = Mathf.Max(maxDamageByHealthType[j], missileWeapon.GetMissileDamage(damageStatsBars[j].HealthType));
                    }
                }
            }
        }

        public override void HideStats()
        {
            base.HideStats();

            speedBar.gameObject.SetActive(false);
            agilityBar.gameObject.SetActive(false);
            rangeBar.gameObject.SetActive(false);

            for (int i = 0; i < damageStatsBars.Count; ++i)
            {
                damageStatsBars[i].gameObject.SetActive(false);
            }
        }

        public override void ShowModuleStats(Module module)
        {
            base.ShowModuleStats(module);

            // If it's a missile, show missile stats
            MissileWeapon missileWeapon = module.GetComponent<MissileWeapon>();
            if (missileWeapon != null)
            {
                for (int i = 0; i < damageStatsBars.Count; ++i)
                {
                    damageStatsBars[i].gameObject.SetActive(true);
                    damageStatsBars[i].SetFillAmount(missileWeapon.GetMissileDamage(damageStatsBars[i].HealthType) /
                                                        maxDamageByHealthType[i]);
                }

                speedBar.gameObject.SetActive(true);
                speedBar.SetFillAmount(missileWeapon.GetMissileSpeed() / maxSpeed);

                agilityBar.gameObject.SetActive(true);
                agilityBar.SetFillAmount(missileWeapon.GetMissileAgility() / maxAgility);

                rangeBar.gameObject.SetActive(true);
                rangeBar.SetFillAmount(missileWeapon.GetMissileRange() / maxRange);
            }
        }
    }
}

