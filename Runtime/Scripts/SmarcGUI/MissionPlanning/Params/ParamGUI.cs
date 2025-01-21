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
        public GameObject ContextMenuPrefab;

        MissionPlanStore missionPlanStore;

        void Awake()
        {
            missionPlanStore = FindFirstObjectByType<MissionPlanStore>();
        }

        public object paramValue
        {
            get => paramsDict!=null? paramsDict[paramKey] : paramsList[paramIndex];
            protected set
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
                var contextMenuGO = Instantiate(ContextMenuPrefab, transform.parent);
                contextMenuGO.GetComponent<RectTransform>().position = eventData.position;
                var contextMenu = contextMenuGO.GetComponent<ListItemContextMenu>();
                contextMenu.SetParam(paramIndex, this);
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

        public void MoveParamUp(int paramIndex)
        {
            if(paramIndex == 0) return;
            var temp = paramsList[paramIndex];
            paramsList[paramIndex] = paramsList[paramIndex-1];
            paramsList[paramIndex-1] = temp;
            missionPlanStore.RefreshTasksGUI();
        }
        
        public void MoveParamDown(int paramIndex)
        {
            if(paramIndex == paramsList.Count-1) return;
            var temp = paramsList[paramIndex];
            paramsList[paramIndex] = paramsList[paramIndex+1];
            paramsList[paramIndex+1] = temp;
            missionPlanStore.RefreshTasksGUI();
        }

        public void DeleteParam(int paramIndex)
        {
            paramsList.RemoveAt(paramIndex);
            missionPlanStore.RefreshTasksGUI();
        }

    }
}