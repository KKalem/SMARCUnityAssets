using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SmarcGUI
{
    public class RobotGUI : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
    {
        [Header("UI Elements")]
        public RectTransform HighlightRT;
        public RectTransform SelectedHighlightRT;
        public GameObject ContextMenuPrefab;
        public TMP_Text RobotNameText;
        public TMP_Text RobotTypeText;

        public string RobotName => RobotNameText.text;

        public bool IsSelected{get; private set;}
        public bool IsSimulated{get; private set;}
        GUIState guiState;

        void Awake()
        {
            guiState = FindFirstObjectByType<GUIState>();
        }


        public void SetSimRobot(GameObject robot)
        {
            IsSimulated = true;
            RobotNameText.text = robot.name;
            RobotTypeText.text = "Simulated";
        }


        public void PingMQTT()
        {
            var pingCommand = new PingCommand();
            guiState.Log($"Sending ping to {RobotName}, {pingCommand.ToJson()}");
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