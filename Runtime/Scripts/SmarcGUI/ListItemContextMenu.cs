using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace SmarcGUI
{
    public class ListItemContextMenu : MonoBehaviour, IPointerExitHandler
    {
        // Task task;
        // MissionPlanStore missionPlanStore;
        Canvas canvas;
        RectTransform rt;
        IListItem item;

        // ParamGUI paramGui;


        public Button DeleteButton;
        public Button MoveUpButton;
        public Button MoveDownButton;

    

        void Awake()
        {
            rt = GetComponent<RectTransform>();
            canvas = FindFirstObjectByType<Canvas>();
        }

        void SetOnTop(Vector2 position)
        {
            rt.SetParent(canvas.transform, false);
            rt.position = position;
            rt.SetAsLastSibling();
        }

        public void SetItem(Vector2 position, IListItem item)
        {
            DeleteButton.onClick.AddListener(OnItemDelete);
            MoveUpButton.onClick.AddListener(OnItemUp);
            MoveDownButton.onClick.AddListener(OnItemDown);
            this.item = item;
            SetOnTop(position);
        }

        void OnItemDelete()
        {
            item.OnListItemDelete();
            Destroy(gameObject);
        }

        void OnItemUp()
        {
            item.OnListItemUp();
            Destroy(gameObject);
        }

        void OnItemDown()
        {
            item.OnListItemDown();
            Destroy(gameObject);
        }
        

        public void OnPointerExit(PointerEventData eventData)
        {
            Destroy(gameObject);
        }

    }
}