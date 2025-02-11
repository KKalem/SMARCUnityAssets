using UnityEngine;
using UnityEngine.UI;


namespace SmarcGUI
{
    public class RobotContextMenu : ContextMenu
    {
        public Button PingButton;

        RobotGUI item;
        public void SetItem(Vector2 position, RobotGUI item)
        {
            this.item = item;
            SetOnTop(position);
            PingButton.onClick.AddListener(OnPing);
        }

        void OnPing()
        {
            item.SendPing();
            Destroy(gameObject);
        }

    }
}
