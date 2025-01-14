using UnityEngine;
using TMPro;
using System.Collections.Generic;

using Utils = DefaultNamespace.Utils;
using VehicleComponents.Sensors;

namespace SmarcGUI
{
    public enum GuiMode
    {
        Monitoring,
        MissionPlanning,
        RemoteControlling
    }

    public class GUIState : MonoBehaviour
    {
        [Header("State")]
        public GuiMode CurrentMode;

        [Header("Colors")]
        public Color ColorMonitoring = Color.green;
        public Color ColorMissionPlanning = Color.red;
        public Color ColorRemoteControlling = Color.blue;


        [Header("GUI Elements")]
        public TMP_Dropdown modeDropdown;
        public TMP_Dropdown cameraDropdown;


        Dictionary<string, string> cameraTextToObjectPath;
        Camera currentCam;


        void InitModeDropdown()
        {
            modeDropdown.onValueChanged.AddListener(OnModeChanged);
            foreach(var modeString in System.Enum.GetNames(typeof(GuiMode)))
            {
                modeDropdown.options.Add(new TMP_Dropdown.OptionData(){text=modeString});
            }

            modeDropdown.value = 0;
            CurrentMode = GuiMode.Monitoring;
            modeDropdown.RefreshShownValue();
        }

        void InitCameraDropdown()
        {
            cameraDropdown.onValueChanged.AddListener(OnCameraChanged);

            cameraTextToObjectPath = new Dictionary<string, string>();
            // disable all cams except the "main cam" at the start
            Camera[] cams = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach(Camera c in cams)
            {
                // dont mess with sensor cameras
                if(c.gameObject.TryGetComponent<Sensor>(out Sensor s)) continue;
                // disable all cams by default. we will enable one later.
                c.enabled = false;
                // disable all audiolisteners. we got no audio. we wont enable these.
                if(c.gameObject.TryGetComponent<AudioListener>(out AudioListener al)) al.enabled=false;
                
                string objectPath = Utils.GetGameObjectPath(c.gameObject);
                string ddText = $"{c.transform.root.name}/{c.name}";
                cameraTextToObjectPath.Add(ddText, objectPath);
                cameraDropdown.options.Add(new TMP_Dropdown.OptionData(){text=ddText});
            }
            cameraDropdown.value = 0;
            cameraDropdown.RefreshShownValue();
            OnCameraChanged(0);
        }

        void Start()
        {
            InitModeDropdown();
            InitCameraDropdown();
        }


        void DrawModeBorder()
        {
            // Display a 1px border around the screen
            GUI.color = CurrentMode switch
            {
                GuiMode.Monitoring => ColorMonitoring,
                GuiMode.MissionPlanning => ColorMissionPlanning,
                GuiMode.RemoteControlling => ColorRemoteControlling,
                _ => Color.white,
            };
            int borderSize = 1;

            // Top border
            GUI.DrawTexture(new Rect(0, 0, Screen.width, borderSize), Texture2D.whiteTexture);
            // Bottom border
            GUI.DrawTexture(new Rect(0, Screen.height - borderSize, Screen.width, borderSize), Texture2D.whiteTexture);
            // Left border
            GUI.DrawTexture(new Rect(0, 0, borderSize, Screen.height), Texture2D.whiteTexture);
            // Right border
            GUI.DrawTexture(new Rect(Screen.width - borderSize, 0, borderSize, Screen.height), Texture2D.whiteTexture);
        }

        void OnGUI()
        {
            DrawModeBorder();
        }

        public void OnModeChanged(int modeIndex)
        {
            var selection = modeDropdown.options[modeIndex];
            CurrentMode = (GuiMode)System.Enum.Parse(typeof(GuiMode), selection.text);
            modeDropdown.value = modeIndex;
            modeDropdown.RefreshShownValue();
        }

        public void OnCameraChanged(int camIndex)
        {
            var selection = cameraDropdown.options[camIndex];
            string objectPath = cameraTextToObjectPath[selection.text];
            GameObject selectedGO = GameObject.Find(objectPath);
            if(selectedGO == null) return;

            if(currentCam != null) currentCam.enabled = false;
            currentCam = selectedGO.GetComponent<Camera>();
            currentCam.enabled = true;
        }

    }
}