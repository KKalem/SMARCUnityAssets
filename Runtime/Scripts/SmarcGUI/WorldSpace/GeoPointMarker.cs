using GeoRef;
using SmarcGUI.MissionPlanning.Params;
using UnityEngine;

namespace SmarcGUI.WorldSpace
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


        Vector3[] circlePoints, pointerPoints;
        int numPtsOnCross = 4; 


        GUIState guiState;

        void Awake()
        {
            globalReferencePoint = FindFirstObjectByType<GlobalReferencePoint>();
            if(globalReferencePoint == null)
            {
                Debug.LogError("GlobalReferencePoint not found, GeoPointMarker will not work, destroying it");
                Destroy(gameObject);
                return;
            }
            guiState = FindFirstObjectByType<GUIState>();
            circlePoints = new Vector3[numPtsOnCircle];
            pointerPoints = new Vector3[2 + numPtsOnCross*2];
        }

        public void SetGeoPointParamGUI(GeoPointParamGUI gppgui)
        {
            this.gppgui = gppgui;
            if(gppgui.altitude == 0 && gppgui.latitude == 0 && gppgui.longitude == 0)
            {
                gameObject.SetActive(false);
                guiState.Log($"GeoPoint {gppgui.name} is not set, hiding the marker");
                return;
            }

            CreateLines();
            OnGUILatLonChanged();
            OnGUIAltChanged();
            UpdateLines();
        }

        void SetLRColor(LineRenderer lr)
        {
            if(transform.position.y < 0) lr.startColor = lr.endColor = UnderwaterColor;
            else if(transform.position.y > 0) lr.startColor = lr.endColor = InAirColor;
            else lr.startColor = lr.endColor = SurfaceColor;
        }

        void SetLRSizes(LineRenderer lr)
        {
            lr.startWidth = lineThickness;
            lr.endWidth = lineThickness;
            lr.material = lineMaterial;
        }

        void CreateLines()
        {
            if(circlePoints == null) return;
            if(pointerPoints == null) return;
            
            for(int i=0; i<numPtsOnCircle; i++)
            {
                float rad = i * 2*Mathf.PI / (numPtsOnCircle-1);
                var x = circleRadius * Mathf.Cos(rad);
                var z = circleRadius * Mathf.Sin(rad);
                circlePoints[i] = new Vector3(x, 0, z);
            }
            Circle.positionCount = circlePoints.Length;
            Circle.SetPositions(circlePoints);
            SetLRColor(Circle);
            SetLRSizes(Circle);

            pointerPoints[0] = new Vector3(0,0,0);
            pointerPoints[1] = new Vector3(0,0,0);
            for(int i=0; i<numPtsOnCross; i++)
            {
                float rad = i * Mathf.PI / 2;
                var x = circleRadius * Mathf.Cos(rad);
                var z = circleRadius * Mathf.Sin(rad);
                pointerPoints[2 + i * 2] = new Vector3(0, 0, 0);
                pointerPoints[2 + i * 2 + 1] = new Vector3(x, 0, z);
            }
            SurfacePointer.positionCount = pointerPoints.Length;
            SurfacePointer.SetPositions(pointerPoints);
            SetLRColor(SurfacePointer);
            SetLRSizes(SurfacePointer);
        }

        public void UpdateLines()
        {            
            for(int i=0; i<numPtsOnCircle; i++)
            {
                Circle.SetPosition(i, circlePoints[i] + transform.position);
            }
            SurfacePointer.SetPosition(0, new Vector3(transform.position.x, 0, transform.position.z));
            SurfacePointer.SetPosition(1, transform.position);
            for(int i=0; i<numPtsOnCross; i++)
            {
                SurfacePointer.SetPosition(2 + i * 2, transform.position);
                SurfacePointer.SetPosition(2 + i * 2 + 1, pointerPoints[2 + i * 2 + 1] + transform.position);
            }

            SetLRColor(Circle);
            SetLRColor(SurfacePointer);
        }

        public void OnWorldDrag(Vector3 motion)
        {
            transform.position += motion;
            UpdateLines();
        }
        

        public void OnWorldDragEnd()
        {
            var (lat, lon) = globalReferencePoint.GetLatLonFromUnityXZ(transform.position.x, transform.position.z);
            gppgui.latitude = lat;
            gppgui.longitude = lon;
            gppgui.altitude = transform.position.y;
        }

        public void OnGUILatLonChanged()
        {
            var (tx,tz) = globalReferencePoint.GetUnityXZFromLatLon(gppgui.latitude, gppgui.longitude);
            transform.position = new Vector3((float)tx, transform.position.y, (float)tz);
            UpdateLines();
        }

        public void OnGUIAltChanged()
        {
            transform.position = new Vector3(transform.position.x, gppgui.altitude, transform.position.z);
            UpdateLines();
        }

        public void ToggleDraggable(bool draggable)
        {
            draggingObject.SetActive(draggable);
        }

    }
}