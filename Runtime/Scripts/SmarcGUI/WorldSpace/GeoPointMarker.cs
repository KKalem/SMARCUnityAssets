using GeoRef;
using UnityEngine;

namespace SmarcGUI
{

    public class GeoPointMarker : MonoBehaviour, IWorldDraggable
    {
        GeoPointParamGUI gppgui;

        public GameObject draggingObject;
        GlobalReferencePoint globalReferencePoint;
        public LineRenderer Circle, SurfacePointer;
        public float circleRadius = 1;
        public int numPtsOnCircle = 50;
        public float lineThickness = 0.1f;
        public Material lineMaterial;
        public Color SurfaceColor = Color.white;
        public Color UnderwaterColor = new(1f,0.4f,0f); //orange
        public Color InAirColor = Color.cyan;


        GUIState guiState;

        void Awake()
        {
            globalReferencePoint = FindFirstObjectByType<GlobalReferencePoint>();
            guiState = FindFirstObjectByType<GUIState>();
        }

        public void SetGeoPointParamGUI(GeoPointParamGUI gppgui)
        {
            this.gppgui = gppgui;
            if(gppgui.altitude == 0 && gppgui.latitude == 0 && gppgui.longitude == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            UpdateLines();
        }

        void SetCommonLRParams(LineRenderer lr)
        {
            lr.startWidth = lineThickness;
            lr.endWidth = lineThickness;
            lr.material = lineMaterial;
            if(gppgui.altitude < 0) lr.startColor = lr.endColor = UnderwaterColor;
            else if(gppgui.altitude > 0) lr.startColor = lr.endColor = InAirColor;
            else lr.startColor = lr.endColor = SurfaceColor;
        }

        public void UpdateLines()
        {
            var (tx,tz) = globalReferencePoint.GetUnityXZFromLatLon(gppgui.latitude, gppgui.longitude);
            transform.position = new Vector3((float)tx, gppgui.altitude, (float)tz);

            var circlePoints = new Vector3[numPtsOnCircle];
            for(int i=0; i<numPtsOnCircle; i++)
            {
                float rad = i * 2*Mathf.PI / (numPtsOnCircle-1);
                var x = transform.position.x + circleRadius * Mathf.Cos(rad);
                var z = transform.position.z + circleRadius * Mathf.Sin(rad);
                circlePoints[i] = new Vector3(x, transform.position.y, z);
            }
            Circle.positionCount = circlePoints.Length;
            Circle.SetPositions(circlePoints);
            SetCommonLRParams(Circle);

            int numPtsOnCross = 4;
            var pointerPoints = new Vector3[2 + numPtsOnCross*2];
            pointerPoints[0] = new Vector3(transform.position.x, 0, transform.position.z);
            pointerPoints[1] = transform.position;
            for(int i=0; i<numPtsOnCross; i++)
            {
                float rad = i * Mathf.PI / 2;
                var x = transform.position.x + circleRadius * Mathf.Cos(rad);
                var z = transform.position.z + circleRadius * Mathf.Sin(rad);
                pointerPoints[2 + i * 2] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                pointerPoints[2 + i * 2 + 1] = new Vector3(x, transform.position.y, z);
            }
            SurfacePointer.positionCount = pointerPoints.Length;
            SurfacePointer.SetPositions(pointerPoints);
            SetCommonLRParams(SurfacePointer);
        }

        public void OnWorldDragEnd(Vector3 motion)
        {
            transform.position += motion;
            var (lat, lon) = globalReferencePoint.GetLatLonFromUnityXZ(transform.position.x, transform.position.z);
            gppgui.latitude = lat;
            gppgui.longitude = lon;
            gppgui.altitude = transform.position.y;
            UpdateLines();
        }

        public void ToggleDraggable(bool draggable)
        {
            draggingObject.SetActive(draggable);
        }

    }
}