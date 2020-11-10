using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VSX.UniversalVehicleCombat
{
    public class ModuleStatsController : MonoBehaviour
    {

        [SerializeField]
        protected Text moduleLabel;

        [SerializeField]
        protected Text moduleDescription;


        public virtual void UpdateMaxStats(List<Module> modulePrefabs)
        {
        }

        public virtual void ShowModuleStats(Module module)
        {
            moduleLabel.gameObject.SetActive(true);
            moduleLabel.text = module.Label;

            moduleDescription.gameObject.SetActive(true);
            moduleDescription.text = module.Description;
        }

        public virtual void HideStats()
        {
            moduleLabel.gameObject.SetActive(false);
            moduleDescription.gameObject.SetActive(false);
        }
    }
}

