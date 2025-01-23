using GeoRef;
using UnityEngine;

namespace SmarcGUI
{
    public class GeoPointMarker : MonoBehaviour
    {
        public GeoPoint gp = new();

        GlobalReferencePoint globalReferencePoint;


        void Awake()
        {
            globalReferencePoint = FindFirstObjectByType<GlobalReferencePoint>();
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
        }
    }
}