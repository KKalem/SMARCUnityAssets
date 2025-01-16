using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;


namespace SmarcGUI
{

    public class TaskParam
    {
        public string Name{get; set;}
        public object Value{get; set;}
    }


    public class Task : IJsonSerializable
    {
        public string Name;
        public string Description;
        public List<TaskParam> Params = new List<TaskParam>();

        public string ToJson()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            fields["name"] = Name;
            fields["description"] = Description;
            fields["params"] = JsonConvert.SerializeObject(Params);
            return JsonConvert.SerializeObject(fields);
        }

        
    }

    
}