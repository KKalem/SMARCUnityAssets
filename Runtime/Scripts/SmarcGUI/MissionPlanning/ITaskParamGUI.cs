using System.Collections.Generic;

namespace SmarcGUI
{
    public interface ITaskParamGUI
    {
        public void SetParam(Dictionary<string, object> allParams, string paramKey);
    }
}