using SmarcGUI.Connections;
using SmarcGUI.MissionPlanning;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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
        public GameObject ContextMenuPrefab;
        public TMP_Text RobotNameText;
        public TMP_Text InfoSourceText;
        InfoSource infoSource;

        public string RobotName => RobotNameText.text;
        string robotNamespace;

        public bool IsSelected{get; private set;}
        GUIState guiState;
        MQTTClientGUI mqttClient;

        void Awake()
        {
            guiState = FindFirstObjectByType<GUIState>();
            mqttClient = FindFirstObjectByType<MQTTClientGUI>();
        }


        public void SetRobot(string robotname, InfoSource infoSource, string robotNamespace)
        {
            this.infoSource = infoSource;
            this.robotNamespace = robotNamespace;

            RobotNameText.text = robotname;
            InfoSourceText.text = $"({infoSource})";

            if(infoSource == InfoSource.SIM) HeartRT.gameObject.SetActive(false);
        }
        


        public void Ping()
        {
            switch(infoSource)
            {
                case InfoSource.SIM:
                    guiState.Log($"Ping! -> {RobotName} in SIM");
                    break;
                case InfoSource.MQTT:
                    var pingCommand = new PingCommand();
                    mqttClient.Publish($"{robotNamespace}/exec/command", pingCommand.ToJson());
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
            var a = msg.SensorDataProvided;
        }

        public void OnGUI()
        {
            HeartRT.localScale = Vector3.Lerp(HeartRT.localScale, Vector3.one, Time.deltaTime * 10);
        }
    }
}