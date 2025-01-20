using UnityEngine;
using TMPro;
using System.Collections.Generic;


namespace SmarcGUI
{

    class GeoPointParamGUI : MonoBehaviour, ITaskParamGUI
    {
        public TMP_InputField LatField, LonField, AltField;
        Dictionary<string, object> allParams;
        string paramKey;

        public void SetParam(Dictionary<string, object> allParams, string paramKey)
        {
            var gp = (GeoPoint)allParams[paramKey];
            LatField.text = gp.latitude.ToString();
            LonField.text = gp.longitude.ToString();
            AltField.text = gp.altitude.ToString();

            this.allParams = allParams;
            this.paramKey = paramKey;

            LatField.onValueChanged.AddListener(OnLatChanged);
            LonField.onValueChanged.AddListener(OnLonChanged);
            AltField.onValueChanged.AddListener(OnAltChanged);
        }

        void OnLatChanged(string s)
        {
            var gp = (GeoPoint)allParams[paramKey];
            gp.latitude = double.Parse(s);
            allParams[paramKey] = gp;
        }

        void OnLonChanged(string s)
        {
            var gp = (GeoPoint)allParams[paramKey];
            gp.longitude = double.Parse(s);
            allParams[paramKey] = gp;
        }   

        void OnAltChanged(string s)
        {
            var gp = (GeoPoint)allParams[paramKey];
            gp.altitude = double.Parse(s);
            allParams[paramKey] = gp;
        }
    }
}
