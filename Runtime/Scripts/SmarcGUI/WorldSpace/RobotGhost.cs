using UnityEngine;

namespace SmarcGUI.WorldSpace
{
    public class RobotGhost : MonoBehaviour
    {
        public Transform ModelTF;
        public float FarAwayDistance = 50;
        float distSq;

        GUIState guiState;

        public Vector3 velocity{ get; private set; }

        void Awake()
        {
            velocity = Vector3.zero;
            distSq = FarAwayDistance * FarAwayDistance;
            guiState = FindFirstObjectByType<GUIState>();
        }

        void FixedUpdate()
        {
            transform.position += velocity * Time.fixedDeltaTime;
            if(velocity.sqrMagnitude < 0.01*0.01)
            {
                velocity = Vector3.zero;
            }
        }

        void LateUpdate()
        {
            if(guiState.CurrentCam == null) return;
            var camDiff = transform.position - guiState.CurrentCam.transform.position;
            bool closeEnough = camDiff.sqrMagnitude < distSq;
            ModelTF.gameObject.SetActive(closeEnough);
        }

        public void Freeze()
        {
            velocity = Vector3.zero;
        }

        public void UpdatePosition(Vector3 pos)
        {
            transform.position = pos;
        }

        public void UpdateHeading(float heading)
        {
            // only visual, so we rotate the 3d model and not the main transform
            ModelTF.transform.rotation = Quaternion.Euler(ModelTF.rotation.eulerAngles.x, heading, ModelTF.transform.rotation.eulerAngles.z);
        }

        public void UpdatePitch(float pitch)
        {
            ModelTF.transform.rotation = Quaternion.Euler(pitch, ModelTF.transform.rotation.eulerAngles.y, ModelTF.transform.rotation.eulerAngles.z);
        }

        public void UpdateRoll(float roll)
        {
            ModelTF.transform.rotation = Quaternion.Euler(ModelTF.transform.rotation.eulerAngles.x, ModelTF.transform.rotation.eulerAngles.y, roll);
        }

        public void UpdateCourse(float course)
        {
            velocity = Quaternion.Euler(0, course, 0) * Vector3.forward * velocity.magnitude;
        }

        public void UpdateSpeed(float speed)
        {
            velocity = velocity.normalized * speed;
        }
    }
}