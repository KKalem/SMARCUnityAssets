using UnityEngine;
using TMPro;
using System.Collections.Generic;

using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NUnit.Framework;

namespace SmarcGUI
{
    public class TaskContextMenu : MonoBehaviour, IPointerExitHandler
    {
        Task task;
        MissionPlanStore missionPlanStore;

        public Button DeleteButton;
        public Button MoveUpButton;
        public Button MoveDownButton;

        void Awake()
        {
            missionPlanStore = FindFirstObjectByType<MissionPlanStore>();
        }

        public void SetTask(Task task)
        {
            this.task = task;
            DeleteButton.onClick.AddListener(OnDeleteTask);
            MoveUpButton.onClick.AddListener(OnUp);
            MoveDownButton.onClick.AddListener(OnDown);
        }

        void OnDeleteTask()
        {
            missionPlanStore.DeleteTask(task);
            Destroy(gameObject);
        }

        void OnUp()
        {
            missionPlanStore.MoveTaskUp(task);
            Destroy(gameObject);
        }

        void OnDown()
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