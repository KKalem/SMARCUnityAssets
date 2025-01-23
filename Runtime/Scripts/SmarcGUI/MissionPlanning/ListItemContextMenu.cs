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
        Canvas canvas;
        RectTransform rt;

        ParamGUI paramGui;


        public Button DeleteButton;
        public Button MoveUpButton;
        public Button MoveDownButton;

        public RectTransform GeoPointExtension;
        public Toggle ShowGeoPointInWorldToggle;

        void Awake()
        {
            missionPlanStore = FindFirstObjectByType<MissionPlanStore>();
            rt = GetComponent<RectTransform>();
            canvas = FindFirstObjectByType<Canvas>();
        }

        void SetOnTop(Vector2 position)
        {
            rt.SetParent(canvas.transform, false);
            rt.position = position;
            rt.SetAsLastSibling();
        }

        // For params in a param-list
        public void SetParam(Vector2 position, ParamGUI paramGui)
        {
            this.paramGui = paramGui;
            DeleteButton.onClick.AddListener(OnParamDelete);
            MoveUpButton.onClick.AddListener(OnParamUp);
            MoveDownButton.onClick.AddListener(OnParamDown);

            SetOnTop(position);
        }

        void OnParamDelete()
        {
            paramGui.DeleteParam();
            Destroy(gameObject);
        }

        void OnParamUp()
        {
            paramGui.MoveParamUp();
            Destroy(gameObject);
        }

        void OnParamDown()
        {
            paramGui.MoveParamDown();
            Destroy(gameObject);
        }

        // For tasks in a task-list
        public void SetTask(Vector2 position, Task task)
        {
            this.task = task;
            DeleteButton.onClick.AddListener(OnTaskDelete);
            MoveUpButton.onClick.AddListener(OnTaskUp);
            MoveDownButton.onClick.AddListener(OnTaskDown);

            SetOnTop(position);
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