using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SmarcGUI
{
    public class TSTGUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler, IListItem
    {
        TaskSpecTree tst;

        [Header("UI Elements")]
        public TMP_InputField DescriptionField;
        public TMP_InputField ExecutionUnitField;
        public RectTransform HighlightRT;
        public RectTransform SelectedHighlightRT;
        public GameObject ContextMenuPrefab;


        [Header("Temp")]
        // TODO read these options from the robot
        public List<string> AvailableTasks = new() { "move-to", "move-path", "custom" };
        

        bool isSelected = false;
        List<TaskGUI> taskGUIs = new();


        MissionPlanStore missionPlanStore;
        GUIState guiState;

        void Awake()
        {
            guiState = FindFirstObjectByType<GUIState>();
            missionPlanStore = FindFirstObjectByType<MissionPlanStore>();
        }


        public void SetTST(TaskSpecTree tst)
        {
            this.tst = tst;

            tst.CommonParams["execution-unit"] = guiState.SelectedRobotName;
            UpdateFieldTexts();
            UpdateTasksDropdown();

            DescriptionField.onValueChanged.AddListener((string desc) => tst.Description = desc);
            ExecutionUnitField.onValueChanged.AddListener((string eu) => tst.CommonParams["execution-unit"] = eu);
        }

        void UpdateFieldTexts()
        {
            DescriptionField.text = tst.Description;
            ExecutionUnitField.text = (string)tst.CommonParams["execution-unit"];
        }

        void UpdateTasksDropdown()
        {
            missionPlanStore.AvailableTasksDropdown.options.Clear();
            if(!isSelected) return;
            foreach (var taskType in AvailableTasks)
            {
                missionPlanStore.AvailableTasksDropdown.options.Add(new TMP_Dropdown.OptionData() { text = taskType });
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HighlightRT.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HighlightRT.gameObject.SetActive(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                var contextMenuGO = Instantiate(ContextMenuPrefab);
                var contextMenu = contextMenuGO.GetComponent<ListItemContextMenu>();
                contextMenu.SetItem(eventData.position, this);
            }

            if(eventData.button == PointerEventData.InputButton.Left)
            {
                isSelected = !isSelected;
                SelectedHighlightRT?.gameObject.SetActive(isSelected);
                missionPlanStore.OnTSTSelected(isSelected? this : null);
                UpdateTasksDropdown();
                UpdateTasksGUI();
            }
        }

        public void OnTaskAdded(int index)
        {
            var taskType = AvailableTasks[index];
            Task newTask = null;
            switch(taskType)
            {
                case "move-to":
                    newTask = new MoveTo("Move to a point", MoveSpeed.STANDARD, new GeoPoint());
                    break;
                case "move-path":
                    newTask = new MovePath("Move along a path", MoveSpeed.STANDARD, new List<GeoPoint>());
                    break;
                case "custom":
                    newTask = new CustomTask("Custom task with a JSON attached", "{\"totally-valid-json\": 42}");
                    break;
            }
            tst.Children.Add(newTask);

            var taskGO = Instantiate(missionPlanStore.TaskPrefab, missionPlanStore.TasksScrollContent);
            var taskGUI = taskGO.GetComponent<TaskGUI>();
            taskGUI.SetTask(newTask);
            taskGUIs.Add(taskGUI);
        }



        // public void DeleteTask(Task task)
        // {
        //     var index = SelectedMissionPlan.Children.IndexOf(task);
        //     if (index >= 0 && index < TasksScrollContent.childCount)
        //     {
        //         SelectedMissionPlan.Children.RemoveAt(index);
        //         Destroy(TasksScrollContent.GetChild(index).gameObject);
        //     }

        // }

        // public void MoveTaskUp(Task task)
        // {
        //     var index = SelectedMissionPlan.Children.IndexOf(task);
        //     if(index == 0) return;
        //     SelectedMissionPlan.Children.RemoveAt(index);
        //     SelectedMissionPlan.Children.Insert(index-1, task);
        //     // Swap the two TaskGUI objects
        //     var taskGO = TasksScrollContent.GetChild(index).gameObject;
        //     var prevTaskGO = TasksScrollContent.GetChild(index - 1).gameObject;
        //     taskGO.transform.SetSiblingIndex(index - 1);
        //     prevTaskGO.transform.SetSiblingIndex(index);
        // }

        // public void MoveTaskDown(Task task)
        // {
        //     var index = SelectedMissionPlan.Children.IndexOf(task);
        //     if(index == SelectedMissionPlan.Children.Count-1) return;
        //     SelectedMissionPlan.Children.RemoveAt(index);
        //     SelectedMissionPlan.Children.Insert(index+1, task);
        //     // Swap the two TaskGUI objects
        //     var taskGO = TasksScrollContent.GetChild(index).gameObject;
        //     var nextTaskGO = TasksScrollContent.GetChild(index + 1).gameObject;
        //     taskGO.transform.SetSiblingIndex(index + 1);
        //     nextTaskGO.transform.SetSiblingIndex(index);
        // }

        void UpdateTasksGUI()
        {
            foreach(Transform child in missionPlanStore.TasksScrollContent)
            {
                child.gameObject.SetActive(false);
            }
            if(!isSelected) return;

            foreach(var taskGUI in taskGUIs)
            {
                taskGUI.gameObject.SetActive(true);
            }
        }




        public void Deselect()
        {
            isSelected = false;
            SelectedHighlightRT?.gameObject.SetActive(isSelected);
            UpdateTasksDropdown();
        }

        public void OnListItemUp()
        {
            missionPlanStore.OnTSTUp(tst);
        }

        public void OnListItemDown()
        {
            missionPlanStore.OnTSTDown(tst);
        }

        public void OnListItemDelete()
        {
            missionPlanStore.OnTSTDelete(tst);
        }
    }
}