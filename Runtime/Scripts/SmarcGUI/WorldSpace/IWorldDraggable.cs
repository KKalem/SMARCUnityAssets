using UnityEngine;

namespace SmarcGUI
{
    public interface IWorldDraggable
    {
        public void OnWorldDragEnd(Vector3 motion);

    }
}