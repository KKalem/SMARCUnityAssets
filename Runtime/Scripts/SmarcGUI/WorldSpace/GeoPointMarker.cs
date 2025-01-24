using GeoRef;
using UnityEngine;

namespace SmarcGUI
{

    public class GeoPointMarker : MonoBehaviour
    {
        public GeoPoint gp = new();

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

        public void SetGeoPoint(GeoPoint gp)
        {
            this.gp = gp;
            if(gp.altitude == 0 && gp.latitude == 0 && gp.longitude == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            var (x,z) = globalReferencePoint.GetUnityXZFromLatLon(gp.latitude, gp.longitude);
            transform.position = new Vector3((float)x, (float)gp.altitude, (float)z);
            UpdateLines();
        }

        void SetCommonLRParams(LineRenderer lr)
        {
            lr.startWidth = lineThickness;
            lr.endWidth = lineThickness;
            lr.material = lineMaterial;
            if(gp.altitude < 0) lr.startColor = lr.endColor = UnderwaterColor;
            else if(gp.altitude > 0) lr.startColor = lr.endColor = InAirColor;
            else lr.startColor = lr.endColor = SurfaceColor;
        }

        void UpdateLines()
        {
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

        
    }
}