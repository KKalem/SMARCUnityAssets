using UnityEngine;
using TMPro;
using System.Collections.Generic;

using Utils = DefaultNamespace.Utils;
using VehicleComponents.Sensors;
using System.IO;
using System;

namespace SmarcGUI
{
    public class BaseTaskGUI : MonoBehaviour
    {
        public Task task;

        public GameObject Params;
        public TMP_InputField DescriptionField;
        public TMP_Text TaskName;
        

        public void SetTask(Task task)
        {
            this.task = task;
            TaskName.text = task.Name;
            DescriptionField.text = task.Description;

            DescriptionField.onValueChanged.AddListener((string desc) => task.Description = desc);
        }


        
    }
}