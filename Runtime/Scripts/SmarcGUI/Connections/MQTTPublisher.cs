using UnityEngine;

namespace SmarcGUI.Connections
{
    public class MQTTPublisher : MonoBehaviour
    {
        protected bool publish = false;

        public virtual void StartPublishing()
        {
            throw new System.NotImplementedException();
        }
        public virtual void StopPublishing()
        {
            throw new System.NotImplementedException();
        }
    }
}