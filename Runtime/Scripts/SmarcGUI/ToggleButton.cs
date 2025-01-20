using UnityEngine;
using UnityEngine.UI;

namespace SmarcGUI
{
    public class ObjectToggler : MonoBehaviour
    {
        public GameObject ToggledObject;

        public bool state = false;

        [Tooltip("If true, the parent will have its UpdateHeight method will be called after toggling")]
        public bool ResizeParent = false;

        Vector2 objectSizeDelta;

        Button button;

        void Start()
        {
            ToggledObject.SetActive(state);
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
            objectSizeDelta = ToggledObject.GetComponent<RectTransform>().sizeDelta;
        }

        public void OnClick()
        {
            state = !state;
            ToggledObject.SetActive(state);
            ToggledObject.GetComponent<RectTransform>().sizeDelta = state? objectSizeDelta : new Vector2(0, 0);

            if(ResizeParent)
            {
                var parent = ToggledObject.transform.parent.GetComponent<IHeightUpdatable>();
                parent.UpdateHeight();
            }
        }
    }
}