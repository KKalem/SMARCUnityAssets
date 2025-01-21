using UnityEngine;
using TMPro;
using System.Collections.Generic;

using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


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


        
        void Awake()
        {
            rt = GetComponent<RectTransform>();
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

            foreach (var param in task.Params)
                InstantiateParam(Params.transform, task.Params, param.Key);

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
            StartCoroutine(UpdateHeightWithDelay());
        }

        IEnumerator UpdateHeightWithDelay()
        {
            yield return new WaitForSeconds(0.01f);
            float totalHeight = 0;
            var paramsRT = Params.GetComponent<RectTransform>();
            totalHeight += paramsRT.sizeDelta.y;
            var nameRT = TaskName.GetComponent<RectTransform>();
            totalHeight += nameRT.sizeDelta.y;
            var descRT = DescriptionField.GetComponent<RectTransform>();
            totalHeight += descRT.sizeDelta.y;

            rt.sizeDelta = new Vector2(rt.sizeDelta.x, totalHeight + BottomPadding);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                var contextMenuGO = Instantiate(ContextMenuPrefab, transform.parent);
                contextMenuGO.GetComponent<RectTransform>().position = eventData.position;
                var contextMenu = contextMenuGO.GetComponent<ListItemContextMenu>();
                contextMenu.SetTask(task);
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
    }
}