using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;


namespace SmarcGUI
{
    [JsonObject(NamingStrategyType = typeof(Newtonsoft.Json.Serialization.KebabCaseNamingStrategy))]
    public class Task
    {
        public string Name;
        public string Description;
        public Dictionary<string, object> Params = new();        

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    
}