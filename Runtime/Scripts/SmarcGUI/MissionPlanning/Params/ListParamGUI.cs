using System.Collections;
using UnityEngine;

namespace SmarcGUI
{
    public class ListParamGUI : ParamGUI, IHeightUpdatable
    {
        RectTransform rt;

        void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        void OnEnable()
        {
            UpdateHeight();
        }

        protected override void SetupFields()
        {
            MissionPlanStore missionPlanStore = FindFirstObjectByType<MissionPlanStore>();
            for(int i=0; i<((IList)paramValue).Count; i++)
            {
                GameObject paramGO;
                GameObject paramPrefab = missionPlanStore.GetParamPrefab(((IList)paramValue)[i]);
                paramGO = Instantiate(paramPrefab, transform);
                paramGO.GetComponent<ParamGUI>().SetParam((IList)paramValue, i);
            }

            UpdateHeight();
        }

        public void UpdateHeight()
        {
            float height = 5;
            foreach(Transform child in transform)
            {
                height += child.GetComponent<RectTransform>().sizeDelta.y;
            }
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        }
    }
}