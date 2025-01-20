using UnityEngine;
using TMPro;
using System.Collections.Generic;

using Utils = DefaultNamespace.Utils;
using VehicleComponents.Sensors;
using System.IO;
using System;

namespace SmarcGUI
{

    class PrimitiveParamGUI : MonoBehaviour
    {
        public TMP_Text LabelText;
        public TMP_InputField InputField;
        public TMP_Dropdown ChoiceDropdown;

        Dictionary<string, object> allParams;
        string paramKey;
        object paramValue{get{return allParams[paramKey];} set{allParams[paramKey] = value;}}

        
        public void SetParam(Dictionary<string, object> allParams, string paramKey)
        {
            this.allParams = allParams;
            this.paramKey = paramKey;

            LabelText.text = paramKey;
            
            switch (paramValue)
            {
                case string s:
                    InputField.gameObject.SetActive(true);
                    InputField.text = s;
                    InputField.contentType = TMP_InputField.ContentType.Standard;
                    InputField.onValueChanged.AddListener(OnInputFieldChanged);
                    break;
                case int i:
                    InputField.gameObject.SetActive(true);
                    InputField.text = i.ToString();
                    InputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                    InputField.onValueChanged.AddListener(OnInputFieldChanged);

                    break;
                case float f:
                    InputField.gameObject.SetActive(true);
                    InputField.text = f.ToString();
                    InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    InputField.onValueChanged.AddListener(OnInputFieldChanged);
                    break;
                case List<string> choices:
                    ChoiceDropdown.gameObject.SetActive(true);
                    ChoiceDropdown.ClearOptions();
                    ChoiceDropdown.AddOptions(choices);
                    break;
                default:
                    InputField.gameObject.SetActive(true);
                    InputField.text = $"Unsupported type: {paramValue.GetType()}";
                    InputField.contentType = TMP_InputField.ContentType.Standard;
                    InputField.interactable = false;
                    break;
            }
        }

        void OnInputFieldChanged(string value)
        {
            switch(InputField.contentType)
            {
                case TMP_InputField.ContentType.Standard:
                    paramValue = value;
                    break;
                case TMP_InputField.ContentType.IntegerNumber:
                    if(int.TryParse(value, out int i))
                        paramValue = i;
                    break;
                case TMP_InputField.ContentType.DecimalNumber:
                    if(float.TryParse(value, out float f))
                        paramValue = f;
                    break;
            }
        }

    }
}
