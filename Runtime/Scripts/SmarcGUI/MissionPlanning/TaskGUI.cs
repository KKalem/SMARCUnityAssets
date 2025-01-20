using UnityEngine;
using TMPro;
using System.Collections.Generic;

using System.Collections;


namespace SmarcGUI
{
    public class TaskGUI : MonoBehaviour, IHeightUpdatable
    {
        public float BottomPadding = 5;
        public Task task;

        public GameObject Params;
        public TMP_InputField DescriptionField;
        public TMP_Text TaskName;


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
                InstantiateParam(Params.transform, param);

            UpdateHeight();
        }

        void InstantiateParam(Transform parent, KeyValuePair<string, object> param)
        {
            GameObject paramGO;
            GameObject paramPrefab = missionPlanStore.GetParamPrefab(param.Value);
            paramGO = Instantiate(paramPrefab, parent);
            paramGO.GetComponent<ITaskParamGUI>().SetParam(task.Params, param.Key);
        }

        public void UpdateHeight()
        {
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


        
    }
}