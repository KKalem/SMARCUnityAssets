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

        protected MissionPlanStore missionPlanStore;
        protected GUIState guiState;
 

        void Awake()
        {
            missionPlanStore = FindFirstObjectByType<MissionPlanStore>();
            guiState = FindFirstObjectByType<GUIState>();
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
                var contextMenuGO = Instantiate(ContextMenuPrefab);
                var contextMenu = contextMenuGO.GetComponent<ListItemContextMenu>();
                contextMenu.SetParam(eventData.position, paramIndex, this);
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
            if(paramsList == null) return;
            if(paramIndex == 0) return;
            (paramsList[paramIndex-1], paramsList[paramIndex]) = (paramsList[paramIndex], paramsList[paramIndex-1]);
            missionPlanStore.RefreshTasksGUI();
        }
        
        public void MoveParamDown(int paramIndex)
        {
            if(paramsList == null) return;
            if(paramIndex == paramsList.Count-1) return;
            (paramsList[paramIndex+1], paramsList[paramIndex]) = (paramsList[paramIndex], paramsList[paramIndex+1]);
            missionPlanStore.RefreshTasksGUI();
        }

        public void DeleteParam(int paramIndex)
        {
            if(paramsList == null) return;
            paramsList.RemoveAt(paramIndex);
            missionPlanStore.RefreshTasksGUI();
        }

    }
}