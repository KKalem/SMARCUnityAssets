using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SmarcGUI
{
    public class ParamGUI : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
    {
        protected IDictionary paramsDict;
        protected string paramKey;

        protected IList paramsList;
        protected int paramIndex;

        public RectTransform HighlightRT;

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


        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                // Show right click menu with options
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HighlightRT?.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HighlightRT?.gameObject.SetActive(true);
        }

    }
}