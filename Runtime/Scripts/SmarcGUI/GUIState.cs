using UnityEngine;
using TMPro;
using System.Collections.Generic;

using Utils = DefaultNamespace.Utils;
using VehicleComponents.Sensors;
using System.IO;
using UnityEngine.EventSystems;

namespace SmarcGUI
{
    public enum GuiMode
    {
        Monitoring,
        MissionPlanning,
        KeyboardControl
    }

    public class GUIState : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        public GuiMode CurrentMode{get; private set;}
        public string SelectedRobotName{get; private set;}
        public string UUID{get; private set;}
        public bool MouseOnGUI{get; private set;}

        [Header("Colors")]
        public Color ColorMonitoring = Color.green;
        public Color ColorMissionPlanning = Color.red;
        public Color ColorRemoteControlling = Color.blue;


        [Header("GUI Elements")]
        public TMP_Dropdown modeDropdown;
        public TMP_Dropdown cameraDropdown;
        public TMP_Dropdown robotDropdown;

        public GameObject KeyboardButtonsPanel;
        public TMP_Text KeyboardButtonsText;

        public TMP_Text LogText;



        [Header("Defaults")]
        public GuiMode DefaultMode = GuiMode.Monitoring;
        public int DefaultRobotIndex = 0;
        public int DefaultCameraIndex = 0;
        public float DefaultCameraLookAtMin = 1;
        public float DefaultCameraLookAtMax = 100;


        Dictionary<string, string> cameraTextToObjectPath;
        public Camera CurrentCam { get; private set; }
        GameObject selectedRobot;
        KeyboardController[] keyboardControllers;



        void InitModeDropdown()
        {
            modeDropdown.onValueChanged.AddListener(OnModeChanged);
            foreach(var modeString in System.Enum.GetNames(typeof(GuiMode)))
            {
                modeDropdown.options.Add(new TMP_Dropdown.OptionData(){text=modeString});
            }

            CurrentMode = DefaultMode;
            modeDropdown.value = (int)DefaultMode;
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

            cameraDropdown.value = DefaultCameraIndex;
            cameraDropdown.RefreshShownValue();
            OnCameraChanged(cameraDropdown.value);
        }

        void InitRobotDropdown()
        {
            robotDropdown.onValueChanged.AddListener(OnRobotChanged);

            var robots = GameObject.FindGameObjectsWithTag("robot");
            if(robots.Length <= 0) return;
            foreach(var robot in robots)
            {
                robotDropdown.options.Add(new TMP_Dropdown.OptionData(){text=robot.name});
            }
            robotDropdown.value = DefaultRobotIndex;
            robotDropdown.RefreshShownValue();
            OnRobotChanged(robotDropdown.value);
        }


        void InitKeyboardControllers()
        {
            keyboardControllers = FindObjectsByType<KeyboardController>(FindObjectsSortMode.None);
            foreach(var k in keyboardControllers) k.enabled = false;
            KeyboardButtonsPanel.SetActive(false);
        }


        void DrawModeBorder()
        {
            // Display a 1px border around the screen
            GUI.color = CurrentMode switch
            {
                GuiMode.Monitoring => ColorMonitoring,
                GuiMode.MissionPlanning => ColorMissionPlanning,
                GuiMode.KeyboardControl => ColorRemoteControlling,
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

            if(CurrentCam != null) CurrentCam.enabled = false;
            CurrentCam = selectedGO.GetComponent<Camera>();
            CurrentCam.enabled = true;
        }
        

        public void OnRobotChanged(int robotIndex)
        {
            SelectedRobotName = robotDropdown.options[robotIndex].text;
            selectedRobot = GameObject.Find(SelectedRobotName);
        }

        public void Log(string text)
        {
            string currentTime = System.DateTime.Now.ToString("HH:mm:ss");
            LogText.text = $"[{currentTime}] {text}\n{LogText.text}";
            if(LogText.text.Length > 5000)
            {
                LogText.text = LogText.text[..1000];
            }
        }


        public Vector3 GetCameraLookAtPoint()
        {
            Ray ray = CurrentCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Plane zeroPlane = new(Vector3.up, Vector3.zero);
            var dist = 10f;
            bool hitWater = false;
            if (zeroPlane.Raycast(ray, out float camToPlaneDist))
            {
                // dont want it too far...
                dist = camToPlaneDist;
                hitWater = true;
            }
            if(!hitWater)
            {
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    dist = hit.distance;
                }
            }
            dist = Mathf.Clamp(dist, DefaultCameraLookAtMin, DefaultCameraLookAtMax);
            return ray.GetPoint(dist);
        }

        void Start()
        {
            UUID = System.Guid.NewGuid().ToString();
            InitKeyboardControllers();
            InitModeDropdown();
            InitCameraDropdown();
            InitRobotDropdown();
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                OnModeChanged((int)DefaultMode);
            }

            if (CurrentMode == GuiMode.KeyboardControl)
            {
                KeyboardButtonsPanel.SetActive(true);
                string text = "";
                text += $"Keyboard Controls: ";
                var kbController = selectedRobot.GetComponentInChildren<KeyboardController>();
                foreach(var kf in kbController.KeysAndFunctions)
                {
                    text += $"{kf.Item1}={kf.Item2}  ";
                }
                KeyboardButtonsText.text = text;
                foreach (var k in keyboardControllers) k.enabled = k.gameObject == selectedRobot;
            }
            else
            {
                KeyboardButtonsPanel.SetActive(false);
                foreach (var k in keyboardControllers) k.enabled = false;
            }
            
        }

        void OnGUI()
        {
            // for things that dont need any interaction, we can draw straight to the screen
            // things like status strings, etc.
            DrawModeBorder();
            // Draw the UUID at the bottom left of the screen
            GUI.color = Color.white;
            GUI.Label(new Rect(0, Screen.height - 20, 400, 20), $"UUID: {UUID}");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseOnGUI = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseOnGUI = true;
        }



    }
}