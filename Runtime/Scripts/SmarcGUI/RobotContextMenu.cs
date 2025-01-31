using UnityEngine;
using UnityEngine.UI;


namespace SmarcGUI
{
    public class RobotContextMenu : ContextMenu
    {
        RobotGUI item;
        public void SetItem(Vector2 position, RobotGUI item)
        {
            this.item = item;
            SetOnTop(position);
        }

    }
}
