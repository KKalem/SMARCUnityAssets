using System;
using System.Collections.Generic;
using GeoRef;
using SmarcGUI.Connections;
using SmarcGUI.MissionPlanning;
using SmarcGUI.MissionPlanning.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SmarcGUI
{
    public enum InfoSource
    {
        SIM,
        MQTT,
        ROS
    }


    public class RobotGUI : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
    {
        [Header("UI Elements")]
        public RectTransform HighlightRT;
        public RectTransform SelectedHighlightRT;
        public RectTransform HeartRT;
        public TMP_Text RobotNameText;
        public TMP_Text InfoSourceText;
        public TMP_Dropdown TasksAvailableDropdown;
        public Button AddTaskButton;
        public RectTransform TasksPanelRT;
        public string WorldMarkerName = "WorldMarkers";

        [Header("Prefabs")]
        public GameObject ContextMenuPrefab;
        public GameObject GenericGhostPrefab;
        public GameObject SAMGhostPrefab;


        Transform worldMarkersTF;
        Transform ghostTF;
        Rigidbody ghostRB;


        public InfoSource InfoSource{get; private set;}
        public WaspDirectExecutionInfoMsg DirectExecutionInfo{get; private set;}
        public List<TaskSpec> TasksAvailable => DirectExecutionInfo.TasksAvailable;
        public List<Dictionary<string, string>> TasksExecuting => DirectExecutionInfo.TasksExecuting;

        public string RobotName => RobotNameText.text;
        string robotNamespace;

        public bool IsSelected{get; private set;}
        GUIState guiState;
        MQTTClientGUI mqttClient;
        GlobalReferencePoint globalReferencePoint;
        MissionPlanStore missionPlanStore;
        RectTransform rt;
        float minHeight;

        void Awake()
        {
            guiState = FindFirstObjectByType<GUIState>();
            mqttClient = FindFirstObjectByType<MQTTClientGUI>();
            missionPlanStore = FindFirstObjectByType<MissionPlanStore>();
            worldMarkersTF = GameObject.Find(WorldMarkerName).transform;
            globalReferencePoint = FindFirstObjectByType<GlobalReferencePoint>();
            AddTaskButton.onClick.AddListener(() => OnTaskAdded(TasksAvailableDropdown.value));
            rt = GetComponent<RectTransform>();
            minHeight = rt.sizeDelta.y;
            TasksPanelRT.gameObject.SetActive(false);
        }

        void UpdateTasksDropdown()
        {
            TasksAvailableDropdown.options.Clear();
            foreach (TaskSpec taskSpec in TasksAvailable)
            {
                TasksAvailableDropdown.options.Add(new TMP_Dropdown.OptionData() { text = taskSpec.Name });
            }
            TasksAvailableDropdown.RefreshShownValue();
        }


        public void SetRobot(string robotname, InfoSource infoSource, string robotNamespace)
        {
            InfoSource = infoSource;
            this.robotNamespace = robotNamespace;

            RobotNameText.text = robotname;
            InfoSourceText.text = $"({infoSource})";

            if(infoSource == InfoSource.SIM) HeartRT.gameObject.SetActive(false);

            if(infoSource == InfoSource.MQTT) 
            {
                mqttClient.SubToTopic(robotNamespace+"sensor_info");
                mqttClient.SubToTopic(robotNamespace+"direct_execution_info");
                mqttClient.SubToTopic(robotNamespace+"sensor/position");
                mqttClient.SubToTopic(robotNamespace+"sensor/heading");
                mqttClient.SubToTopic(robotNamespace+"sensor/course");
                mqttClient.SubToTopic(robotNamespace+"sensor/speed");
                TasksPanelRT.gameObject.SetActive(true);
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, minHeight + TasksPanelRT.sizeDelta.y);
            }

            if(infoSource != InfoSource.SIM && worldMarkersTF != null)
            {
                if(robotname.ToLower().Contains("sam")) ghostTF = Instantiate(SAMGhostPrefab).transform;
                else ghostTF = Instantiate(GenericGhostPrefab).transform;
                ghostTF.SetParent(worldMarkersTF);
                ghostTF.gameObject.SetActive(false);
                ghostRB = ghostTF.GetComponent<Rigidbody>();
            }
        }
        


        public void Ping()
        {
            switch(InfoSource)
            {
                case InfoSource.SIM:
                    guiState.Log($"Ping! -> {RobotName} in SIM");
                    break;
                case InfoSource.MQTT:
                    var pingCommand = new PingCommand();
                    mqttClient.Publish(robotNamespace+"exec/command", pingCommand.ToJson());
                    break;
                case InfoSource.ROS:
                    guiState.Log($"Ping! -> {RobotName} in ROS");
                    break;
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                var contextMenuGO = Instantiate(ContextMenuPrefab);
                var contextMenu = contextMenuGO.GetComponent<RobotContextMenu>();
                contextMenu.SetItem(eventData.position, this);
            }

            if(eventData.button == PointerEventData.InputButton.Left)
            {
                IsSelected = !IsSelected;
                OnSelectedChange(true);
            }
        }

        void OnSelectedChange(bool notify = false)
        {
            SelectedHighlightRT?.gameObject.SetActive(IsSelected);
            if(notify) guiState.OnRobotSelectionChanged(this);
        }

        public void Deselect()
        {
            IsSelected = false;
            OnSelectedChange();
        }

        void OnTaskAdded(int index)
        {
            var taskSpec = TasksAvailable[index];
            missionPlanStore.SelectedTSTGUI?.OnTaskAdded(taskSpec);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HighlightRT?.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HighlightRT?.gameObject.SetActive(true);
        }

        public void OnHeartbeatReceived()
        {
            HeartRT.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

        public void OnSensorInfoReceived(WaspSensorInfoMsg msg)
        {
            return;
        }

        public void OnDirectExecutionInfoReceived(WaspDirectExecutionInfoMsg msg)
        {
            DirectExecutionInfo = msg;
            UpdateTasksDropdown();
        }

        public void OnPositionReceived(GeoPoint pos)
        {
            if(globalReferencePoint == null) return;
            if(ghostTF == null) return;
            if(ghostTF.gameObject.activeSelf == false) ghostTF.gameObject.SetActive(true);
            var (x,z) = globalReferencePoint.GetUnityXZFromLatLon(pos.latitude, pos.longitude);
            ghostTF.position = new Vector3(x, pos.altitude, z);
        }

        public void OnHeadingReceived(float heading)
        {
            ghostTF.rotation = Quaternion.Euler(0, heading, 0);
        }

        public void OnCourseReceived(float course)
        {
            var speed = ghostRB.linearVelocity.magnitude;
            // waraps really isnt made for things that move in 3D space, so we'll just set the velocity in the xz plane...
            ghostRB.linearVelocity = speed * new Vector3(Mathf.Sin(course * Mathf.Deg2Rad), 0, Mathf.Cos(course * Mathf.Deg2Rad));
        }

        public void OnSpeedReceived(float speed)
        {
            ghostRB.linearVelocity = ghostRB.linearVelocity.normalized * speed;
        }

        public void OnGUI()
        {
            HeartRT.localScale = Vector3.Lerp(HeartRT.localScale, Vector3.one, Time.deltaTime * 10);
            AddTaskButton.interactable = missionPlanStore.SelectedTSTGUI != null;
        }

    }
}