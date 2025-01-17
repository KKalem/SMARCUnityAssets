using UnityEngine;
using TMPro;
using System.Collections.Generic;

// using Utils = DefaultNamespace.Utils;
using VehicleComponents.Sensors;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEditor.EditorTools;
using UnityEngine.UI;


namespace SmarcGUI
{
    [RequireComponent(typeof(GUIState))]
    public class MisionPlanStore : MonoBehaviour
    {
        GUIState guiState;

        [Tooltip("Path to store mission plans")]
        public string MissionStoragePath;
        public List<TaskSpecTree> MissionPlans = new();

        [Header("GUI Elements")]
        public TMP_Dropdown MissionPlanDropdown;
        public TMP_InputField MissionPlanNameField;
        public Button NewMissionPlanButton;
        public Button DeleteMissionPlanButton;
        public TMP_Dropdown AvailableTasksDropdown;
        public Button AddTaskButton;
        public Transform TasksScrollContent;

        [Header("Prefabs")]
        public GameObject TaskTemplatePrefab;


        // TODO read these options from the robot
        public List<string> AvailableTasks = new() { "move-to", "move-path", "custom" };



        void LoadMissionPlans()
        {
            var i=0;
            foreach (var file in Directory.GetFiles(MissionStoragePath))
            {
                if(!file.EndsWith(".json")) continue;
                var json = File.ReadAllText(file);
                try
                {
                    var plan = JsonConvert.DeserializeObject<TaskSpecTree>(json);
                    MissionPlans.Add(plan);
                    i++;
                }
                catch (Exception e)
                {
                    print($"Failed to load mission plan from {file}: {e.Message}");
                    continue;
                }
            }
            print($"Loaded {i} mission plans");
        }

        void InitGUIElements()
        {
            MissionPlanDropdown.onValueChanged.AddListener(OnMissionPlanChanged);
            foreach (var plan in MissionPlans)
            {
                MissionPlanDropdown.options.Add(new TMP_Dropdown.OptionData() { text = plan.Name });
            }
            NewMissionPlanButton.onClick.AddListener(OnNewMissionPlan);
            DeleteMissionPlanButton.onClick.AddListener(OnDeleteMissionPlan);
            MissionPlanNameField.onEndEdit.AddListener(OnMissionPlanNameChanged);

            foreach(var taskType in AvailableTasks)
            {
                AvailableTasksDropdown.options.Add(new TMP_Dropdown.OptionData() { text = taskType });
            }
            AddTaskButton.onClick.AddListener(() => OnTaskAdded(AvailableTasksDropdown.value));
        }

        void RefreshNamesToLastMission()
        {
            MissionPlanDropdown.value = MissionPlans.Count==0? -1 : MissionPlans.Count - 1;
            MissionPlanDropdown.RefreshShownValue();
            MissionPlanNameField.text = MissionPlans.Count == 0 ? "NO MISSION PLAN" : MissionPlans[MissionPlanDropdown.value].Name;
        }

        public void OnNewMissionPlan()
        {
            var newPlan = new TaskSpecTree();
            newPlan.Name = "Unnamed Mission Plan";

            MissionPlans.Add(newPlan);
            MissionPlanDropdown.options.Add(new TMP_Dropdown.OptionData() { text = newPlan.Name });
            RefreshNamesToLastMission();
            RefreshTasksGUI();
        }

        public void OnDeleteMissionPlan()
        {
            if(MissionPlans.Count == 0) return;
            var index = MissionPlanDropdown.value;
            MissionPlans.RemoveAt(index);
            MissionPlanDropdown.options.RemoveAt(index);
            RefreshNamesToLastMission();
            RefreshTasksGUI();
        }

        public void OnMissionPlanNameChanged(string name)
        {
            var index = MissionPlanDropdown.value;
            MissionPlans[index].Name = name;
            MissionPlanDropdown.options[index].text = name;
            MissionPlanDropdown.RefreshShownValue();
        }

        void OnMissionPlanChanged(int index)
        {
            var plan = MissionPlans[index];
            MissionPlanNameField.text = plan.Name;
            RefreshTasksGUI();
        }

        void OnTaskAdded(int index)
        {
            if(MissionPlans.Count == 0) return;
            if(MissionPlanDropdown.value < 0) return;
            var plan = MissionPlans[MissionPlanDropdown.value];
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
                    newTask = new CustomTask("Custom task with a JSON attached", "{}");
                    break;
            }
            plan.Children.Add(newTask);
            RefreshTasksGUI();
        }

        void RefreshTasksGUI()
        {
            foreach(Transform child in TasksScrollContent)
            {
                Destroy(child.gameObject);
            }
            if(MissionPlans.Count == 0) return;
            var plan = MissionPlans[MissionPlanDropdown.value];
            foreach(var task in plan.Children)
            {
                var taskGO = Instantiate(TaskTemplatePrefab, TasksScrollContent);
                var taskGUI = taskGO.GetComponent<BaseTaskGUI>();
                taskGUI.SetTask(task);
            }
        }
        

        void Awake()
        {
            guiState = GetComponent<GUIState>();

            // Desktop on win, user home on linux/mac
            MissionStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Path.Combine("SMaRCUnity", "MissionPlans"));
            Directory.CreateDirectory(MissionStoragePath);
            LoadMissionPlans();

            InitGUIElements();
        }

        void OnGUI()
        {
            bool enabled = false;
            if(MissionPlans.Count > 0)
            {
                enabled = MissionPlans[MissionPlanDropdown.value] != null;
            }
            AvailableTasksDropdown.interactable = enabled;
            AddTaskButton.interactable = enabled;
            DeleteMissionPlanButton.interactable = enabled;
            MissionPlanNameField.interactable = enabled;

        }

    }
}