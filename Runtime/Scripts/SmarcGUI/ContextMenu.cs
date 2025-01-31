using UnityEngine;
using UnityEngine.EventSystems;


namespace SmarcGUI
{
    public class ContextMenu : MonoBehaviour, IPointerExitHandler
    {
        Canvas canvas;
        RectTransform rt;

        void Awake()
        {
            rt = GetComponent<RectTransform>();
            canvas = FindFirstObjectByType<Canvas>();
        }

        protected void SetOnTop(Vector2 position)
        {
            rt.SetParent(canvas.transform, false);
            rt.position = position;
            rt.SetAsLastSibling();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Destroy(gameObject);
        }

    }
}
