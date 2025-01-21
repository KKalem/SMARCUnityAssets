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

        IList paramsList;
        int paramIndex;

        public Button DeleteButton;
        public Button MoveUpButton;
        public Button MoveDownButton;

        void Awake()
        {
            missionPlanStore = FindFirstObjectByType<MissionPlanStore>();
        }


        // For params in a param-list
        public void SetParam(IList paramsList, int paramIndex)
        {
            this.paramsList = paramsList;
            this.paramIndex = paramIndex;
            DeleteButton.onClick.AddListener(OnParamDelete);
            MoveUpButton.onClick.AddListener(OnParamUp);
            MoveDownButton.onClick.AddListener(OnParamDown);
        }

        void OnParamDelete()
        {
            paramsList.RemoveAt(paramIndex);
            Destroy(gameObject);
            missionPlanStore.RefreshTasksGUI();
        }

        void OnParamUp()
        {
            if(paramIndex == 0) return;
            var param = paramsList[paramIndex];
            paramsList.RemoveAt(paramIndex);
            paramsList.Insert(paramIndex-1, param);
            Destroy(gameObject);
            missionPlanStore.RefreshTasksGUI();
        }

        void OnParamDown()
        {
            if(paramIndex == paramsList.Count-1) return;
            var param = paramsList[paramIndex];
            paramsList.RemoveAt(paramIndex);
            paramsList.Insert(paramIndex+1, param);
            Destroy(gameObject);
            missionPlanStore.RefreshTasksGUI();
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