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
        public GameObject ContextMenuPrefab;
        public TMP_Text RobotNameText;
        public TMP_Text InfoSourceText;
        InfoSource infoSource;

        public string RobotName => RobotNameText.text;

        public bool IsSelected{get; private set;}
        GUIState guiState;

        void Awake()
        {
            guiState = FindFirstObjectByType<GUIState>();
        }


        public void SetRobot(string robotname, InfoSource infoSource)
        {
            this.infoSource = infoSource;
            RobotNameText.text = robotname;
            InfoSourceText.text = infoSource.ToString();
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
                    guiState.Log($"Pinging {RobotName} through MQTT, {pingCommand.ToJson()}");
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
                OnSelectedChange();
            }
        }

        void OnSelectedChange()
        {
            SelectedHighlightRT?.gameObject.SetActive(IsSelected);
            guiState.OnRobotSelectionChanged(this);
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

        
    }
}