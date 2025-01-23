using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace SmarcGUI
{
    public class ListParamGUI : ParamGUI, IHeightUpdatable
    {
        RectTransform rt;

        public RectTransform content;
        public TMP_Text Label;
        public Button AddButton;

        IList theList => (IList)paramValue;

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

            AddButton.onClick.AddListener(AddParamToList);

            UpdateHeight();
        }

        void AddParamToList()
        {
            if (theList is null)
                return;

            // Assuming theList contains elements of a specific type, e.g., ParamType
            // if this is not the case, something has gone horribly wrong on the
            // TaskSpecTree side of things.
            // This aint python, lists usually cant contain arbitrary mixes of types
            var paramType = theList.GetType().GetGenericArguments()[0];
            var newParam = System.Activator.CreateInstance(paramType);

            theList.Add(newParam);

            // Instantiate the new parameter GUI")
            missionPlanStore ??= FindFirstObjectByType<MissionPlanStore>();
            GameObject paramPrefab = missionPlanStore.GetParamPrefab(newParam);
            GameObject paramGO = Instantiate(paramPrefab, content);
            paramGO.GetComponent<ParamGUI>().SetParam(theList, math.max(0, theList.Count - 1));

            UpdateHeight();
            transform.parent.GetComponentInParent<IHeightUpdatable>().UpdateHeight();
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