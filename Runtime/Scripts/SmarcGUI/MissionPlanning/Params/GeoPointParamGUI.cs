using GeoRef;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;



namespace SmarcGUI
{


    public class GeoPointParamGUI : ParamGUI
    {
        public TMP_InputField LatField, LonField, AltField;

        public GameObject WorldMarkerPrefab;
        public string WorldMarkersName = "WorldMarkers";

        GeoPointMarker worldMaker;
        GlobalReferencePoint globalReferencePoint;

        public float altitude
        {
            get{return (float)((GeoPoint)paramValue).altitude; }
            set{
                var gp = (GeoPoint)paramValue;
                gp.altitude = value;
                paramValue = gp;
            }
        }
        public double latitude
        {
            get{return ((GeoPoint)paramValue).latitude; }
            set{
                var gp = (GeoPoint)paramValue;
                gp.latitude = value;
                paramValue = gp;
            }
        }
        public double longitude
        {
            get{return ((GeoPoint)paramValue).longitude; }
            set{
                var gp = (GeoPoint)paramValue;
                gp.longitude = value;
                paramValue = gp;
            }
        }

        void Awake()
        {
            globalReferencePoint = FindFirstObjectByType<GlobalReferencePoint>();
            guiState = FindFirstObjectByType<GUIState>();
        }

        protected override void SetupFields()
        {
            if(altitude == 0 && latitude == 0 && longitude == 0)
            {
                // set this to be the same as the previous geo point
                if (paramIndex > 0)
                {
                    var previousGp = (GeoPoint)paramsList[paramIndex - 1];
                    latitude = previousGp.latitude;
                    longitude = previousGp.longitude;
                    altitude = previousGp.altitude;
                    guiState.Log("New GeoPoint set to previous.");
                }
                // if there is no previous geo point, set it to where the camera is looking at
                else
                {
                    var point = guiState.GetCameraLookAtPoint();
                    var (lat, lon) = globalReferencePoint.GetLatLonFromUnityXZ(point.x, point.z);
                    latitude = lat;
                    longitude = lon;
                    altitude = point.y;
                    guiState.Log("New GeoPoint set to where the camera is looking at.");
                }
            }

            UpdateTexts();

            LatField.onValueChanged.AddListener(OnLatChanged);
            LonField.onValueChanged.AddListener(OnLonChanged);
            AltField.onValueChanged.AddListener(OnAltChanged);

            var WorldMarkers = GameObject.Find(WorldMarkersName).transform;
            worldMaker = Instantiate(WorldMarkerPrefab, WorldMarkers).GetComponent<GeoPointMarker>();
            worldMaker.SetGeoPointParamGUI(this);
        }

        public void UpdateTexts()
        {
            LatField.text = latitude.ToString();
            LonField.text = longitude.ToString();
            AltField.text = altitude.ToString();
        }

        void OnLatChanged(string s)
        {
            try {latitude = double.Parse(s);}
            catch {return;}
            worldMaker.UpdateLines();
        }

        void OnLonChanged(string s)
        {
            try{longitude = double.Parse(s);}
            catch{return;}
            worldMaker.UpdateLines();
        }   

        void OnAltChanged(string s)
        {
            try{altitude = float.Parse(s);}
            catch{return;}
            worldMaker.UpdateLines();
        }

        void OnDestroy()
        {
            Destroy(worldMaker);
        }
    }
}
