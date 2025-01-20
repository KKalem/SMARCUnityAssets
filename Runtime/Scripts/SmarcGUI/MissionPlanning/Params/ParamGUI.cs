using System.Collections;
using UnityEngine;

namespace SmarcGUI
{
    public class ParamGUI : MonoBehaviour
    {
        protected IDictionary paramsDict;
        protected string paramKey;

        protected IList paramsList;
        protected int paramIndex;

        protected object paramValue
        {
            get => paramsDict!=null? paramsDict[paramKey] : paramsList[paramIndex];
            set
            {
                if(paramsDict!=null)
                    paramsDict[paramKey] = value;
                else
                    paramsList[paramIndex] = value;
            }
        }

        public void SetParam(IDictionary paramsDict, string paramKey)
        {
            this.paramsDict = paramsDict;
            this.paramKey = paramKey;
            SetupFields();
        }
        public void SetParam(IList paramsList, int paramIndex)
        {   
            this.paramsList = paramsList;
            this.paramIndex = paramIndex;
            SetupFields();
        }

        protected virtual void SetupFields()
        {
            throw new System.NotImplementedException();
        }

    }
}