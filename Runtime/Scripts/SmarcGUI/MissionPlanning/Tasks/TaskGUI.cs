using UnityEngine;
using TMPro;
using System.Collections.Generic;

using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;


namespace SmarcGUI
{
    public class TaskGUI : MonoBehaviour, IHeightUpdatable, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
    {
        public float BottomPadding = 5;
        public Task task;

        public GameObject Params;
        public TMP_InputField DescriptionField;
        public TMP_Text TaskName;
        public RectTransform HighlightRT;

        public GameObject ContextMenuPrefab;


        MissionPlanStore missionPlanStore;
        RectTransform rt;
        Canvas canvas;

        bool needsHeightUpdate = false;


        
        void Awake()
        {
            rt = GetComponent<RectTransform>();
            canvas = transform.root.GetComponentInChildren<Canvas>();
        }

        void OnEnable()
        {
            UpdateHeight();
        }

        public void SetTask(Task task, MissionPlanStore store)
        {
            missionPlanStore = store;
            this.task = task;
            TaskName.text = task.Name;
            DescriptionField.text = task.Description;

            DescriptionField.onValueChanged.AddListener((string desc) => task.Description = desc);

            // instead of a foreach, we need to iterate over index because the param itself could modify the
            // individual parameter at this point
            for(int i=0; i<task.Params.Count; i++)
                InstantiateParam(Params.transform, task.Params, task.Params.Keys.ElementAt(i));

            UpdateHeight();
        }

        void InstantiateParam(Transform parent, Dictionary<string, object> taskParams, string paramKey)
        {
            GameObject paramGO;
            GameObject paramPrefab = missionPlanStore.GetParamPrefab(taskParams[paramKey]);
            paramGO = Instantiate(paramPrefab, parent);
            paramGO.GetComponent<ParamGUI>().SetParam(taskParams, paramKey);
        }


        public void UpdateHeight()
        {
            // Why? because this is under a scroll view and we cant have size-fitter component without problems
            // this seems to let the scroll view do its thing, and then update the size after.
            // Basically delaying the update by one frame.
            needsHeightUpdate = true;
        }

        void ActuallyUpdateHeight()
        {
            float totalHeight = 0;
            var paramsRT = Params.GetComponent<RectTransform>();
            totalHeight += paramsRT.sizeDelta.y;
            var nameRT = TaskName.GetComponent<RectTransform>();
            totalHeight += nameRT.sizeDelta.y;
            var descRT = DescriptionField.GetComponent<RectTransform>();
            totalHeight += descRT.sizeDelta.y;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, totalHeight + BottomPadding);
            needsHeightUpdate = false;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                var contextMenuGO = Instantiate(ContextMenuPrefab);
                var contextMenu = contextMenuGO.GetComponent<ListItemContextMenu>();
                contextMenu.SetTask(eventData.position, task);
            }

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HighlightRT.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HighlightRT.gameObject.SetActive(true);
        }

        public void OnGUI()
        {
            if(needsHeightUpdate) ActuallyUpdateHeight();
        }

        public void OnDisable()
        {
            foreach (Transform child in Params.transform)
            {
                Destroy(child.gameObject);
            }
        }

    }
}