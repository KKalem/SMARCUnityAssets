using System.Collections.Generic;
using UnityEngine;

namespace SmarcGUI
{
    public interface IPathInWorld
    {
        List<Vector3> GetWorldPath();
    }
}