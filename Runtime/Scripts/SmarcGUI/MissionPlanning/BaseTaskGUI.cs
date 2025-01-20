using UnityEngine;
using TMPro;
using System.Collections.Generic;

using Utils = DefaultNamespace.Utils;
using VehicleComponents.Sensors;
using System.IO;
using System;
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

        RectTransform rt;
        Vector2 baseSizeDelta;

        
        void Awake()
        {
            rt = GetComponent<RectTransform>();
            baseSizeDelta = rt.sizeDelta;
        }

        public void SetTask(Task task)
        {
            this.task = task;
            TaskName.text = task.Name;
            DescriptionField.text = task.Description;

            DescriptionField.onValueChanged.AddListener((string desc) => task.Description = desc);

            foreach (var param in task.Params)
            {
                var paramGO = Instantiate(PrimitiveParamPrefab, Params.transform);
                paramGO.GetComponent<PrimitiveParamGUI>().SetParam(task.Params, param.Key);
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