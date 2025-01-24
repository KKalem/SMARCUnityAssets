using GeoRef;
using TMPro;
using UnityEngine;



namespace SmarcGUI
{


    class GeoPointParamGUI : ParamGUI
    {
        public TMP_InputField LatField, LonField, AltField;

        public GameObject WorldMarkerPrefab;
        public string WorldMarkersName = "WorldMarkers";

        GameObject worldMaker;
        GlobalReferencePoint globalReferencePoint;

        void Awake()
        {
            globalReferencePoint = FindFirstObjectByType<GlobalReferencePoint>();
            guiState = FindFirstObjectByType<GUIState>();
        }

        protected override void SetupFields()
        {
            var gp = (GeoPoint)paramValue;
            if(gp.altitude == 0 && gp.latitude == 0 && gp.longitude == 0)
            {
                // set this to be the same as the previous geo point
                if (paramIndex > 0)
                {
                    var previousGp = (GeoPoint)paramsList[paramIndex - 1];
                    gp.latitude = previousGp.latitude;
                    gp.longitude = previousGp.longitude;
                    gp.altitude = previousGp.altitude;
                    paramValue = gp;
                    guiState.Log("New GeoPoint set to previous.");
                }
                // if there is no previous geo point, set it to where the camera is looking at
                else
                {
                    var point = guiState.GetCameraLookAtPoint();
                    var (lat, lon) = globalReferencePoint.GetLatLonFromUnityXZ(point.x, point.z);
                    gp.latitude = lat;
                    gp.longitude = lon;
                    gp.altitude = point.y;
                    paramValue = gp;
                    guiState.Log("New GeoPoint set to where the camera is looking at.");
                }
            }

            LatField.text = gp.latitude.ToString();
            LonField.text = gp.longitude.ToString();
            AltField.text = gp.altitude.ToString();

            LatField.onValueChanged.AddListener(OnLatChanged);
            LonField.onValueChanged.AddListener(OnLonChanged);
            AltField.onValueChanged.AddListener(OnAltChanged);

            var WorldMarkers = GameObject.Find(WorldMarkersName).transform;
            worldMaker = Instantiate(WorldMarkerPrefab, WorldMarkers);
            worldMaker.GetComponent<GeoPointMarker>().SetGeoPoint(gp);
        }

        void OnLatChanged(string s)
        {
            var gp = (GeoPoint)paramValue;
            try
            {
                gp.latitude = double.Parse(s);
            }
            catch
            {
                return;
            }
            paramValue = gp;
            worldMaker.GetComponent<GeoPointMarker>().SetGeoPoint(gp);
        }

        void OnLonChanged(string s)
        {
            var gp = (GeoPoint)paramValue;
            try
            {
                gp.longitude = double.Parse(s);
            }
            catch
            {
                return;
            }
            paramValue = gp;
            worldMaker.GetComponent<GeoPointMarker>().SetGeoPoint(gp);
        }   

        void OnAltChanged(string s)
        {
            var gp = (GeoPoint)paramValue;
            try
            {
                gp.altitude = double.Parse(s);
            }
            catch
            {
                return;
            }
            paramValue = gp;
            worldMaker.GetComponent<GeoPointMarker>().SetGeoPoint(gp);
        }

        void OnDestroy()
        {
            Destroy(worldMaker);
        }
    }
}
