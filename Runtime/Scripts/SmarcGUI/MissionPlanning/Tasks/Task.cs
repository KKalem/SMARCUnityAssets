using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;


namespace SmarcGUI
{
    public class Task : IJsonSerializable
    {
        public string Name;
        public string Description;
        public Dictionary<string, object> Params = new();

        public string ToJson()
        {
            // we're playing this weird game here because wasp uses "params" as a key in their json
            // but we can't use it as a variable name in C# because it's a reserved keyword...
            // so we cant just replicate the json structure in C# straight and use Serialize(this) lol
            Dictionary<string, object> fields = new()
            {
                ["name"] = Name,
                ["description"] = Description,
                ["params"] = Params
            };
            return JsonConvert.SerializeObject(fields);
        }

        
    }

    
}