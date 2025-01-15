using UnityEngine;

namespace SmarcGUI
{
    public interface IRobotChangedListener
    {
        void OnRobotChanged(GameObject newRobot);
    }
}