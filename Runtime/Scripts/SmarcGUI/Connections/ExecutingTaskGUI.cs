using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;



namespace SmarcGUI.Connections
{
    public class ExecutingTaskGUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        [Header("UI Elements")]
        public TMP_Text TaskName;
        public TMP_Dropdown TaskSignalsDropdown;
        public Button SignalButton;
        public RectTransform HighlightRT;

        RobotGUI robotgui;
        string taskUuid;

        public void SetExecTask(RobotGUI robotgui, string taskName, string taskUuid, List<string> signals)
        {
            this.robotgui = robotgui;
            TaskName.text = taskName;
            this.taskUuid = taskUuid;

            TaskSignalsDropdown.ClearOptions();
            TaskSignalsDropdown.AddOptions(signals);
            SignalButton.onClick.AddListener(OnSignalButton);
        }

        void OnSignalButton()
        {
            robotgui.SendSignalCommand(taskUuid, TaskSignalsDropdown.options[TaskSignalsDropdown.value].text);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HighlightRT.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HighlightRT.gameObject.SetActive(true);
        }
    }
}