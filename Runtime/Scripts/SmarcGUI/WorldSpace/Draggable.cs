using UnityEngine;
using UnityEngine.EventSystems;

namespace SmarcGUI
{
    public enum DragConstraint
    {
        X,
        Y,
        Z,
        XY,
        XZ,
        YZ,
        NONE
    }

    public class Draggable : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public DragConstraint DragConstraint = DragConstraint.XZ;
        public Transform DraggedObject;

        Vector3 motion;

        GUIState guiState;

        void Awake()
        {
            guiState = FindFirstObjectByType<GUIState>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            // https://gist.github.com/SimonDarksideJ/477f5674285b63cba8e752c43950ed7c
            Ray camRay = guiState.CurrentCam.ScreenPointToRay(Input.mousePosition); // Get the ray from mouse position
            Vector3 planeOrigin = transform.position; // Take current position of this draggable object as Plane's Origin
            Vector3 planeNormal = -guiState.CurrentCam.transform.forward; // Take current negative camera's forward as Plane's Normal
            Plane plane = new(planeNormal, planeOrigin); // Create a plane with the normal and origin.
            plane.Raycast(camRay, out float camPlaneDist); // Find the intersection point.
            Vector3 newPos = camRay.origin + camRay.direction * camPlaneDist; // Find the new point.
            // Apply constraints
            switch (DragConstraint)
            {
                case DragConstraint.X:
                    newPos.y = transform.position.y;
                    newPos.z = transform.position.z;
                    break;
                case DragConstraint.Y:
                    newPos.x = transform.position.x;
                    newPos.z = transform.position.z;
                    break;
                case DragConstraint.Z:
                    newPos.x = transform.position.x;
                    newPos.y = transform.position.y;
                    break;
                case DragConstraint.XY:
                    newPos.z = transform.position.z;
                    break;
                case DragConstraint.XZ:
                    newPos.y = transform.position.y;
                    break;
                case DragConstraint.YZ:
                    newPos.x = transform.position.x;
                    break;
                case DragConstraint.NONE:
                    break;
            }

            if (DraggedObject != null)
            {
                motion = newPos - transform.position;
                DraggedObject.position += motion;
            }
            else
            {
                transform.position = newPos;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DraggedObject?.GetComponent<IWorldDraggable>()?.OnWorldDragEnd(motion);
            motion = Vector3.zero;
        }
    }
}
