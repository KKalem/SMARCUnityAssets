using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace SmarcGUI
{
    public class ListItemContextMenu : MonoBehaviour, IPointerExitHandler
    {
        Task task;
        MissionPlanStore missionPlanStore;

        ParamGUI paramGui;
        int paramIndex;


        public Button DeleteButton;
        public Button MoveUpButton;
        public Button MoveDownButton;

        public RectTransform GeoPointExtension;
        public Toggle ShowGeoPointInWorldToggle;

        void Awake()
        {
            missionPlanStore = FindFirstObjectByType<MissionPlanStore>();
        }


        // For params in a param-list
        public void SetParam(int paramIndex, ParamGUI paramGui)
        {
            this.paramGui = paramGui;
            this.paramIndex = paramIndex;
            DeleteButton.onClick.AddListener(OnParamDelete);
            MoveUpButton.onClick.AddListener(OnParamUp);
            MoveDownButton.onClick.AddListener(OnParamDown);
        }

        void OnParamDelete()
        {
            paramGui.DeleteParam(paramIndex);
            Destroy(gameObject);
        }

        void OnParamUp()
        {
            paramGui.MoveParamUp(paramIndex);
            Destroy(gameObject);
        }

        void OnParamDown()
        {
            paramGui.MoveParamDown(paramIndex);
            Destroy(gameObject);
        }

        // For tasks in a task-list
        public void SetTask(Task task)
        {
            this.task = task;
            DeleteButton.onClick.AddListener(OnTaskDelete);
            MoveUpButton.onClick.AddListener(OnTaskUp);
            MoveDownButton.onClick.AddListener(OnTaskDown);
        }

        void OnTaskDelete()
        {
            missionPlanStore.DeleteTask(task);
            Destroy(gameObject);
        }

        void OnTaskUp()
        {
            missionPlanStore.MoveTaskUp(task);
            Destroy(gameObject);
        }

        void OnTaskDown()
        {
            missionPlanStore.MoveTaskDown(task);
            Destroy(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Destroy(gameObject);
        }

    }
}