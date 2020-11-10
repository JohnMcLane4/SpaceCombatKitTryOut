using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public enum BoolEvaluationType
    {
        IsEqualTo
    }


    [System.Serializable]
    public class Condition
    {

        [SerializeField]
        protected LinkableVariable condition;

        [SerializeField]
        protected BoolEvaluationType boolEvaluationType;

        [SerializeField]
        protected bool boolReferenceValue;

        public void Initialize()
        {
            condition.InitializeLinkDelegate();
        }

        public bool ConditionMet()
        {
            switch (boolEvaluationType)
            {
                case BoolEvaluationType.IsEqualTo:
                    return condition.BoolValue == boolReferenceValue;
                default:
                    return true;
            }
        }

        public bool ConditionValue()
        {
            return condition.BoolValue;
        }
    }

    public enum BooleanConditionsEvaluationType
    {
        And,
        Or
    }

    [System.Serializable]
    public class Conditions
    {

        [SerializeField]
        protected BooleanConditionsEvaluationType evaluationType;

        [SerializeField]
        protected List<Condition> conditionsList = new List<Condition>();

        public void Initialize()
        {
            for(int i = 0; i < conditionsList.Count; ++i)
            {
                conditionsList[i].Initialize();
            }
        }

        public bool ConditionsMet
        {
            get
            {
                switch (evaluationType)
                {
                    case BooleanConditionsEvaluationType.And:
                        for (int i = 0; i < conditionsList.Count; ++i)
                        {
                            if (!conditionsList[i].ConditionMet())
                            {
                                return false;
                            }
                        }

                        return true;

                    case BooleanConditionsEvaluationType.Or:
                        for (int i = 0; i < conditionsList.Count; ++i)
                        {
                            if (conditionsList[i].ConditionMet())
                            {
                                return true;
                            }
                        }
                        return true;
                }

                return false;
            }
        }

        public List<bool> ConditionsValues()
        {
            List<bool> conditionValues = new List<bool>();
            for (int i = 0; i < conditionsList.Count; ++i)
            {
                conditionValues.Add(conditionsList[i].ConditionValue());
            }
            return conditionValues;
        }
    }
}

