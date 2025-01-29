using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SmarcGUI
{
    public class TSTGUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler, IListItem, IPathInWorld, IPathChangeListener
    {
        TaskSpecTree tst;

        [Header("UI Elements")]
        public TMP_InputField DescriptionField;
        public TMP_InputField ExecutionUnitField;
        public RectTransform HighlightRT;
        public RectTransform SelectedHighlightRT;
        public GameObject ContextMenuPrefab;
        LineRenderer PathLineRenderer;


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
            PathLineRenderer = GetComponent<LineRenderer>();
        }


        public void SetTST(TaskSpecTree tst)
        {
            this.tst = tst;

            tst.CommonParams["execution-unit"] = guiState.SelectedRobotName;
            UpdateFieldTexts();
            UpdateTasksDropdown();
            UpdateTasksGUI();

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
            CreateTaskGUI(newTask);
        }

        void CreateTaskGUI(Task task)
        {
            var taskGO = Instantiate(missionPlanStore.TaskPrefab, missionPlanStore.TasksScrollContent);
            var taskGUI = taskGO.GetComponent<TaskGUI>();
            taskGUI.SetTask(task, this);
            taskGUIs.Add(taskGUI);
        }



        public void DeleteTask(TaskGUI taskgui)
        {
            var index = tst.Children.IndexOf(taskgui.task);
            tst.Children.RemoveAt(index);
            Destroy(taskgui.gameObject);
            taskGUIs.Remove(taskgui);
            OnPathChanged();
        }

        public void MoveTaskUp(TaskGUI taskgui)
        {
            var index = tst.Children.IndexOf(taskgui.task);
            if(index == 0) return;
            tst.Children.RemoveAt(index);
            tst.Children.Insert(index-1, taskgui.task);
            // Swap the two TaskGUI objects
            var guiIndex = taskgui.transform.GetSiblingIndex();
            taskgui.transform.SetSiblingIndex(guiIndex - 1);
            OnPathChanged();
        }

        public void MoveTaskDown(TaskGUI taskgui)
        {
            var index = tst.Children.IndexOf(taskgui.task);
            if(index == tst.Children.Count-1) return;
            tst.Children.RemoveAt(index);
            tst.Children.Insert(index+1, taskgui.task);
            // Swap the two TaskGUI objects
            var guiIndex = taskgui.transform.GetSiblingIndex();
            taskgui.transform.SetSiblingIndex(guiIndex + 1);
            OnPathChanged();
        }

        void UpdateTasksGUI()
        {
            // maybe the first time creating this gui
            if(taskGUIs.Count == 0)
            {
                foreach(var task in tst.Children)
                {
                    CreateTaskGUI(task);
                }
            }

            foreach(Transform child in missionPlanStore.TasksScrollContent)
            {
                child.gameObject.SetActive(false);
            }
            if(!isSelected) return;
            foreach(var taskGUI in taskGUIs)
            {
                taskGUI.gameObject.SetActive(true);
            }
            OnPathChanged();
        }
        public void OnDisable()
        {
            foreach (var taskGUI in taskGUIs)
            {
                taskGUI.gameObject.SetActive(false);
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

        public List<Vector3> GetWorldPath()
        {
            var path = new List<Vector3>();
            foreach(var taskGUI in taskGUIs)
            {
                path.AddRange(taskGUI.GetWorldPath());
            }
            return path;
        }

        void DrawWorldPath()
        {
            var path = GetWorldPath();
            PathLineRenderer.positionCount = path.Count;
            PathLineRenderer.SetPositions(path.ToArray());
        }

        public void OnPathChanged()
        {
            DrawWorldPath();
        }
    }
}