using UnityEngine;
using UnityEngine.UI;

namespace SmarcGUI
{
    public class ObjectToggler : MonoBehaviour
    {
        public GameObject ToggledObject;

        public bool state = false;

        Button button;

        void Start()
        {
            ToggledObject.SetActive(state);
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            state = !state;
            ToggledObject.SetActive(state);
        }
    }
}