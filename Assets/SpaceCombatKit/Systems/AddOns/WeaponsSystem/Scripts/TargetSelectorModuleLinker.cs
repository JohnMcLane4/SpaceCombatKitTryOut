using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{
    public class TargetSelectorModuleLinker : MonoBehaviour
    {

        [SerializeField]
        protected TargetSelector targetSelector;

        [SerializeField]
        protected ModuleMount moduleMount;


        protected void Awake()
        {
            moduleMount.onModuleMounted.AddListener(OnModuleMounted);
            moduleMount.onModuleUnmounted.AddListener(OnModuleUnmounted);
        }

        protected void OnModuleMounted(Module module)
        {
            TargetLeader targetLeader = module.GetComponent<TargetLeader>();
            if (targetLeader != null)
            {
                targetSelector.onSelectedTargetChanged.AddListener(targetLeader.SetTarget);
            }

            TargetLocker targetLocker = module.GetComponent<TargetLocker>();
            if (targetLocker != null)
            {
                targetSelector.onSelectedTargetChanged.AddListener(targetLocker.SetTarget);
            }
        }

        protected void OnModuleUnmounted(Module module)
        {
            TargetLeader targetLeader = module.GetComponent<TargetLeader>();
            if (targetLeader != null)
            {
                targetSelector.onSelectedTargetChanged.RemoveListener(targetLeader.SetTarget);
            }

            TargetLocker targetLocker = module.GetComponent<TargetLocker>();
            if (targetLocker != null)
            {
                targetSelector.onSelectedTargetChanged.RemoveListener(targetLocker.SetTarget);
            }
        }
    }
}

