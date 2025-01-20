using UnityEngine;
using TMPro;
using System.Collections.Generic;

using System.Collections;


namespace SmarcGUI
{
    public class BaseTaskGUI : MonoBehaviour, IHeightUpdatable
    {
        public float BottomPadding = 5;
        public Task task;

        public GameObject Params;
        public TMP_InputField DescriptionField;
        public TMP_Text TaskName;

        public GameObject PrimitiveParamPrefab;
        public GameObject GeoPointParamPrefab;

        RectTransform rt;
        Vector2 baseSizeDelta;

        
        void Awake()
        {
            rt = GetComponent<RectTransform>();
            baseSizeDelta = rt.sizeDelta;
        }

        void OnEnable()
        {
            UpdateHeight();
        }

        public void SetTask(Task task)
        {
            this.task = task;
            TaskName.text = task.Name;
            DescriptionField.text = task.Description;

            DescriptionField.onValueChanged.AddListener((string desc) => task.Description = desc);

            foreach (var param in task.Params)
            {
                var paramVal = param.Value;
                GameObject paramGO;
                switch(paramVal)
                {
                    case string s:
                    case int i:
                    case float f:
                    case List<string> choices:
                        paramGO = Instantiate(PrimitiveParamPrefab, Params.transform);
                        paramGO.GetComponent<PrimitiveParamGUI>().SetParam(task.Params, param.Key);
                        break;
                    case GeoPoint gp:
                        paramGO = Instantiate(GeoPointParamPrefab, Params.transform);
                        paramGO.GetComponent<GeoPointParamGUI>().SetParam(task.Params, param.Key);
                        break;
                    default:
                        return;
                }
                paramGO.SetActive(true);
            }
            UpdateHeight();
        }

        public void UpdateHeight()
        {
            StartCoroutine(UpdateHeightWithDelay());
        }

        IEnumerator UpdateHeightWithDelay()
        {
            yield return new WaitForSeconds(0.01f);
            var paramsRT = Params.GetComponent<RectTransform>();
            rt.sizeDelta = baseSizeDelta + new Vector2(0, paramsRT.sizeDelta.y) + new Vector2(0, BottomPadding);
        }


        
    }
}