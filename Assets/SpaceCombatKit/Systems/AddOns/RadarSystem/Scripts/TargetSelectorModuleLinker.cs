using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Radar
{
    [DefaultExecutionOrder(-50)]
    public class TargetSelectorModuleLinker : MonoBehaviour
    {

        [SerializeField]
        protected TargetSelector targetSelector;

        [SerializeField]
        protected List<ModuleMount> moduleMounts = new List<ModuleMount>();
        

        protected void Awake()
        {
            for (int i = 0; i < moduleMounts.Count; ++i)
            {
                moduleMounts[i].onModuleMounted.AddListener(OnModuleMounted);
                moduleMounts[i].onModuleUnmounted.AddListener(OnModuleUnmounted);
            }
        }

        protected void OnModuleMounted(Module module)
        {
            if (targetSelector != null)
            {
                TargetLocker[] targetLockers = module.GetComponentsInChildren<TargetLocker>();
                for (int i = 0; i < targetLockers.Length; ++i)
                {
                    targetLockers[i].SetTarget(targetSelector.SelectedTarget);
                    targetSelector.onSelectedTargetChanged.AddListener(targetLockers[i].SetTarget);
                }

                TargetLeader[] targetLeaders = module.GetComponentsInChildren<TargetLeader>();
                for (int i = 0; i < targetLeaders.Length; ++i)
                {
                    targetLeaders[i].SetTarget(targetSelector.SelectedTarget);
                    targetSelector.onSelectedTargetChanged.AddListener(targetLeaders[i].SetTarget);
                }
            }
        }

        protected void OnModuleUnmounted(Module module)
        {
            if (targetSelector != null)
            {
                TargetLocker[] targetLockers = module.GetComponentsInChildren<TargetLocker>();
                for (int i = 0; i < targetLockers.Length; ++i)
                {
                    targetSelector.onSelectedTargetChanged.RemoveListener(targetLockers[i].SetTarget);
                }

                TargetLeader[] targetLeaders = module.GetComponentsInChildren<TargetLeader>();
                for (int i = 0; i < targetLeaders.Length; ++i)
                {
                    targetSelector.onSelectedTargetChanged.RemoveListener(targetLeaders[i].SetTarget);
                }
            }
        }
    }
}
