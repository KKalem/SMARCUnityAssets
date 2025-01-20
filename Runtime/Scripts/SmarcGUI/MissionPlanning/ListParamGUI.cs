using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UI;

namespace SmarcGUI
{
    public class ListParamGUI : MonoBehaviour, IHeightUpdatable, ITaskParamGUI
    {
        Dictionary<string, object> allParams;
        string paramKey;


        public void SetParam(Dictionary<string, object> allParams, string paramKey)
        {
            this.allParams = allParams;
            this.paramKey = paramKey;

            List<object> paramsInList = (List<object>)allParams[paramKey];
            for(int i=0; i<paramsInList.Count; i++)
            {
                var kv = new KeyValuePair<string, object>(i.ToString(), paramsInList[i]);
                
            }
                    

            UpdateHeight();
        }

        public void UpdateHeight()
        {
            
        }
    }
}