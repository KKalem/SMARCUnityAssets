using UnityEngine;

namespace SmarcGUI
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SimpleMeshToggle : WaterRenderToggle
    {
        MeshRenderer meshRenderer;

        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public override void ToggleWaterRender(bool render)
        {
            meshRenderer.enabled = render;
        }
    }
}