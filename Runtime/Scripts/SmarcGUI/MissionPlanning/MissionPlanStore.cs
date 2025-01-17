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
    public class MisionPlanStore : MonoBehaviour
    {
        [Tooltip("Path to store mission plans")]
        public string MissionStoragePath;
        public List<TaskSpecTree> MissionPlans = new();

        [Header("GUI Elements")]
        public TMP_Dropdown MissionPlanDropdown;
        public TMP_InputField MissionPlanNameField;

        public Button NewMissionPlanButton;
        public Button DeleteMissionPlanButton;


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
        }

        public void OnDeleteMissionPlan()
        {
            if(MissionPlans.Count == 0) return;
            var index = MissionPlanDropdown.value;
            MissionPlans.RemoveAt(index);
            MissionPlanDropdown.options.RemoveAt(index);
            RefreshNamesToLastMission();
           
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
        }

        void Awake()
        {
            // Desktop on win, user home on linux/mac
            MissionStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Path.Combine("SMaRCUnity", "MissionPlans"));
            Directory.CreateDirectory(MissionStoragePath);
            LoadMissionPlans();

            InitGUIElements();
        }



    }
}