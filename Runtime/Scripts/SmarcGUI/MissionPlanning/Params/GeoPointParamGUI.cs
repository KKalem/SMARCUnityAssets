using TMPro;



namespace SmarcGUI
{

    class GeoPointParamGUI : ParamGUI
    {
        public TMP_Text Label;
        public TMP_InputField LatField, LonField, AltField;

        protected override void SetupFields()
        {
            Label.text = paramKey ?? paramIndex.ToString();

            var gp = (GeoPoint)paramValue;
            LatField.text = gp.latitude.ToString();
            LonField.text = gp.longitude.ToString();
            AltField.text = gp.altitude.ToString();

            LatField.onValueChanged.AddListener(OnLatChanged);
            LonField.onValueChanged.AddListener(OnLonChanged);
            AltField.onValueChanged.AddListener(OnAltChanged);
        }

        void OnLatChanged(string s)
        {
            var gp = (GeoPoint)paramValue;
            gp.latitude = double.Parse(s);
            paramValue = gp;
        }

        void OnLonChanged(string s)
        {
            var gp = (GeoPoint)paramValue;
            gp.longitude = double.Parse(s);
            paramValue = gp;
        }   

        void OnAltChanged(string s)
        {
            var gp = (GeoPoint)paramValue;
            gp.altitude = double.Parse(s);
            paramValue = gp;
        }
    }
}
