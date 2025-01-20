using System.Collections;
using TMPro;
using UnityEngine;

namespace SmarcGUI
{
    public class ListParamGUI : ParamGUI, IHeightUpdatable
    {
        RectTransform rt;

        public RectTransform content;
        public TMP_Text Label;

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
            Label.text = paramKey ?? paramIndex.ToString();
            
            MissionPlanStore missionPlanStore = FindFirstObjectByType<MissionPlanStore>();
            for(int i=0; i<((IList)paramValue).Count; i++)
            {
                GameObject paramGO;
                GameObject paramPrefab = missionPlanStore.GetParamPrefab(((IList)paramValue)[i]);
                paramGO = Instantiate(paramPrefab, content);
                paramGO.GetComponent<ParamGUI>().SetParam((IList)paramValue, i);
            }

            UpdateHeight();
        }

        public void UpdateHeight()
        {
            float contentHeight = 5;
            foreach(Transform child in content)
                contentHeight += child.GetComponent<RectTransform>().sizeDelta.y;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
            
            float selfHeight = 5;
            foreach(Transform child in transform)
                selfHeight += child.GetComponent<RectTransform>().sizeDelta.y;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, selfHeight);
        }
    }
}