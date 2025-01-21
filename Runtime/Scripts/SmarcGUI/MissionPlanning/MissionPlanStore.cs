using UnityEngine;
using TMPro;
using System.Collections.Generic;

using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Collections;
using Codice.Client.Common.GameUI;


namespace SmarcGUI
{
    [RequireComponent(typeof(GUIState))]
    public class MissionPlanStore : MonoBehaviour
    {
        GUIState guiState;

        [Tooltip("Path to store mission plans")]
        public string MissionStoragePath;
        public List<TaskSpecTree> MissionPlans = new();

        TaskSpecTree SelectedMissionPlan => MissionPlans[MissionPlanDropdown.value];

        [Header("GUI Elements")]
        public TMP_Dropdown MissionPlanDropdown;
        public TMP_InputField MissionPlanNameField;
        public Button NewMissionPlanButton;
        public Button LoadMissionsButton;
        public Button SaveMissionsButton;
        public Button DeleteMissionPlanButton;
        public TMP_Dropdown AvailableTasksDropdown;
        public Button AddTaskButton;
        public Transform TasksScrollContent;

        [Header("Prefabs")]
        public GameObject TaskPrefab;
        public GameObject PrimitiveParamPrefab;
        public GameObject GeoPointParamPrefab;
        public GameObject ListParamPrefab;


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
                    // Json does not know about _classes_ so we need to recover the types
                    // by checking for simple fields, and matching them to known classes
                    // Most of the work is done in the Task class
                    plan.RecoverFromJson();
                    MissionPlans.Add(plan);
                    i++;
                }
                catch (Exception e)
                {
                    guiState.Log($"Failed to load mission plan from {file}: {e.Message}");
                    continue;
                }
            }
            guiState.Log($"Loaded {i} mission plans");
            OnMissionPlanChanged(0);
        }

        void SaveMissionPlans()
        {
            var i=0;
            foreach (var plan in MissionPlans)
            {
                var json = JsonConvert.SerializeObject(plan, Formatting.Indented);
                var path = Path.Combine(MissionStoragePath, $"{plan.Name}.json");
                File.WriteAllText(path, json);
                i++;
            }
            guiState.Log($"Saved {i} mission plans");
        }

        void RefreshMissionPlansDropdown()
        {
            MissionPlanDropdown.options.Clear();
            foreach (var plan in MissionPlans)
            {
                MissionPlanDropdown.options.Add(new TMP_Dropdown.OptionData() { text = plan.Name });
            }
        }

        void RefreshAvailableTasksDropdown()
        {
            AvailableTasksDropdown.options.Clear();
            foreach (var taskType in AvailableTasks)
            {
                AvailableTasksDropdown.options.Add(new TMP_Dropdown.OptionData() { text = taskType });
            }
        }

        void InitGUIElements()
        {
            MissionPlanDropdown.onValueChanged.AddListener(OnMissionPlanChanged);
            NewMissionPlanButton.onClick.AddListener(OnNewMissionPlan);
            DeleteMissionPlanButton.onClick.AddListener(OnDeleteMissionPlan);
            LoadMissionsButton.onClick.AddListener(LoadMissionPlans);
            SaveMissionsButton.onClick.AddListener(SaveMissionPlans);
            MissionPlanNameField.onEndEdit.AddListener(OnMissionPlanNameChanged);
            AddTaskButton.onClick.AddListener(() => OnTaskAdded(AvailableTasksDropdown.value));

            RefreshAvailableTasksDropdown();
            RefreshMissionPlansDropdown();
            RefreshTasksGUI();
        }

        void RefreshNamesToLastMission()
        {
            MissionPlanDropdown.value = MissionPlans.Count==0? -1 : MissionPlans.Count - 1;
            MissionPlanDropdown.RefreshShownValue();
            MissionPlanNameField.text = MissionPlans.Count == 0 ? "NO MISSION PLAN" : SelectedMissionPlan.Name;
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
            var taskType = AvailableTasks[index];
            Task newTask = null;
            switch(taskType)
            {
                case "move-to":
                    newTask = new MoveTo("Move to a point", MoveSpeed.STANDARD, new GeoPoint(){latitude=1, longitude=2, altitude=3});
                    break;
                case "move-path":
                    var list = new List<GeoPoint>
                    {
                        new() { latitude = 0, longitude = 0, altitude = 0 },
                        new() { latitude = 0, longitude = 0, altitude = 1 },
                        new() { latitude = 0, longitude = 0, altitude = 2 }
                    };
                    newTask = new MovePath("Move along a path", MoveSpeed.STANDARD, list);
                    break;
                case "custom":
                    newTask = new CustomTask("Custom task with a JSON attached", "{amazing-thing: 42}");
                    break;
            }
            SelectedMissionPlan.Children.Add(newTask);
            RefreshTasksGUI();
        }

        public void DeleteTask(Task task)
        {
            SelectedMissionPlan.Children.Remove(task);
            RefreshTasksGUI();
        }

        public void MoveTaskUp(Task task)
        {
            var index = SelectedMissionPlan.Children.IndexOf(task);
            if(index == 0) return;
            SelectedMissionPlan.Children.RemoveAt(index);
            SelectedMissionPlan.Children.Insert(index-1, task);
            RefreshTasksGUI();
        }

        public void MoveTaskDown(Task task)
        {
            var index = SelectedMissionPlan.Children.IndexOf(task);
            if(index == SelectedMissionPlan.Children.Count-1) return;
            SelectedMissionPlan.Children.RemoveAt(index);
            SelectedMissionPlan.Children.Insert(index+1, task);
            RefreshTasksGUI();
        }

        public void RefreshTasksGUI()
        {
            foreach(Transform child in TasksScrollContent)
            {
                Destroy(child.gameObject);
            }
            if(MissionPlans.Count == 0) return;
            var plan = SelectedMissionPlan;
            foreach(var task in plan.Children)
            {
                var taskGO = Instantiate(TaskPrefab, TasksScrollContent);
                var taskGUI = taskGO.GetComponent<TaskGUI>();
                taskGUI.SetTask(task, this);
            }
        }

        public GameObject GetParamPrefab(object paramValue)
        {
            return paramValue switch
            {
                string or int or float or bool => PrimitiveParamPrefab,
                GeoPoint => GeoPointParamPrefab,
                IList => ListParamPrefab,
                _ => PrimitiveParamPrefab,
            };
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
                enabled = SelectedMissionPlan != null;
            }
            AvailableTasksDropdown.interactable = enabled;
            AddTaskButton.interactable = enabled;
            DeleteMissionPlanButton.interactable = enabled;
            MissionPlanNameField.interactable = enabled;
            MissionPlanDropdown.interactable = enabled;

        }

    }
}