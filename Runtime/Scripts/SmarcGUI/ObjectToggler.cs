using UnityEngine;
using UnityEngine.UI;

namespace SmarcGUI
{
    public class ObjectToggler : MonoBehaviour
    {
        public GameObject ToggledObject;
        [Tooltip("If true, the object being toggled will have its UpdateHeight method called after toggling")]
        public bool ResizeObject = true;

        [Tooltip("Default state of the object at startup")]    
        public bool state = false;

        [Tooltip("If true, the parent will have its UpdateHeight method called after toggling")]
        public bool ResizeParent = false;


        [Tooltip("If set, the object will have its UpdateHeight method called after toggling")]
        public GameObject ResizeParentObject;


        Vector2 objectSizeDelta;
        Button button;

        void Start()
        {
            ToggledObject.SetActive(state);
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
            objectSizeDelta = ToggledObject.GetComponent<RectTransform>().sizeDelta;
            Resize();
        }

        void Resize()
        {
            if(ResizeObject)
            {
                var heightUpdatable = ToggledObject.GetComponent<IHeightUpdatable>();
                heightUpdatable?.UpdateHeight();
            }

            if(ResizeParent)
            {
                var parent = ToggledObject.transform.parent.GetComponent<IHeightUpdatable>();
                parent?.UpdateHeight();
            }

            if(ResizeParentObject)
            {
                var parent = ResizeParentObject.GetComponent<IHeightUpdatable>();
                parent?.UpdateHeight();
            }
        }

        public void OnClick()
        {
            state = !state;
            ToggledObject.SetActive(state);
            ToggledObject.GetComponent<RectTransform>().sizeDelta = state? objectSizeDelta : new Vector2(0, 0);
            Resize();
        }
    }
}