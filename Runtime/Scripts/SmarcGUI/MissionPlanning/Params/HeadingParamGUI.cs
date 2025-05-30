using SmarcGUI.WorldSpace;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace SmarcGUI.MissionPlanning.Params
{
    public class HeadingParamGUI : ParamGUI, IParamHasHeading
    {
        [Header("HeadingParamGUI")]
        public TMP_InputField HeadingField;

        public float heading
        {
            get { return ((Heading)paramValue).heading; }
            set
            {
                var d = (Heading)paramValue;
                d.heading = value;
                paramValue = d;
                HeadingField.text = value.ToString();
                NotifyPathChange();
            }
        }

        protected override void SetupFields()
        {
            heading = 0;
            HeadingField.text = heading.ToString();
            HeadingField.onEndEdit.AddListener(value => SetHeading(float.Parse(value)));
            fields.Add(HeadingField.GetComponent<RectTransform>());
        }

        public override List<string> GetFieldLabels()
        {
            return new List<string> { "Heading" };
        }

        public float GetHeading()
        {
            return heading;
        }

        public void SetHeading(float heading)
        {
            this.heading = heading;
        }
    }
}
