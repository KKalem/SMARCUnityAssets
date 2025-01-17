using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;


namespace SmarcGUI
{
    //https://api-docs.waraps.org/#/agent_communication/tasks/commands

    public class Command : IJsonSerializable
    {
        public Dictionary<string, object> fields = new();
        public string ToJson()
        {
            return JsonConvert.SerializeObject(fields);
        }
    }

    public class PingCommand : Command
    {
        public PingCommand()
        {
            fields["command"] = "ping";
            fields["com-uuid"] = Guid.NewGuid().ToString();
            fields["sender"] = "UnityGUI";
        }
    }

    public class SigntalTaskCommand: Command
    {
        public static string ENOUGH{ get{return "$enough";} }
        public static string PAUSE{ get{return "$pause";} }
        public static string CONTINUE{ get{return "$continue";} }
        public static string ABORT{ get{return "$abort";} }


        public SigntalTaskCommand(string signal)
        {
            fields["command"] = "signal-task";
            fields["com-uuid"] = Guid.NewGuid().ToString();
            fields["sender"] = "UnityGUI";

            fields["signal"] = signal;
            fields["task-uuid"] = Guid.NewGuid().ToString();
        }

    }

    public class StartTaskCommand : Command
    {
        public string execution_unit;
        public Task task;
        public string task_uuid;

        public StartTaskCommand(Task task, string robot_name)
        {
            fields["command"] = "start-task";
            fields["com-uuid"] = Guid.NewGuid().ToString();
            fields["sender"] = "UnityGUI";

            fields["execution_unit"] = robot_name;
            fields["task-uuid"] = Guid.NewGuid().ToString();
            fields["task"] = task;    
        }
        

    }

    // Defined but no docs on this, so ignoring for now.
    // public class QueryStatusCommand : Command
    // {

    // }

}