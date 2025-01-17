using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmarcGUI
{
    [JsonObject(NamingStrategyType = typeof(Newtonsoft.Json.Serialization.KebabCaseNamingStrategy))]
    public class TaskSpecTree
    {
        // the TST definition of waraps is... complex. 99% of the time we(smarc) wont be using the _entire_ spec
        // so im just implementing the basic "list of tasks" here. Which corresponds to their "L3" agents.
        // That means, this is a "linear tree" where the root is a sequence and thats it.
        // if anyone wants the _entire_ L4 spec, i wish you good luck :)
        public Dictionary<string, object> CommonParams;
        public string Name;
        public Dictionary<string, object> Params;
        public List<Task> Children;
    }
}