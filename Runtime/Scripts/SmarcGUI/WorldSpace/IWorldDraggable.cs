using UnityEngine;

namespace SmarcGUI
{
    public interface IWorldDraggable
    {
        public void OnWorldDrag(Vector3 newPos);
        public void OnWorldDragEnd();

    }
}