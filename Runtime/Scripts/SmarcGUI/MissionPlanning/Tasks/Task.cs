using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


namespace SmarcGUI
{
    [JsonObject(NamingStrategyType = typeof(Newtonsoft.Json.Serialization.KebabCaseNamingStrategy))]
    public class Task
    {
        public string Name{get; protected set;}
        public string Description;
        public Dictionary<string, object> Params = new();

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
       
    }

    
}