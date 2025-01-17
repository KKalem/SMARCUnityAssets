using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;


namespace SmarcGUI
{
    //https://api-docs.waraps.org/#/agent_communication/tasks/commands

    [JsonObject(NamingStrategyType = typeof(Newtonsoft.Json.Serialization.KebabCaseNamingStrategy))]
    public class BaseCommand
    {
        public string Command;
        public string ComUuid;
        public string Sender = "UnityGUI";

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class PingCommand : BaseCommand
    {
        public PingCommand()
        {
            Command = "ping";
            ComUuid = Guid.NewGuid().ToString();
        }
    }

    public class SigntalTaskCommand: BaseCommand
    {
        public static string ENOUGH{ get{return "$enough";} }
        public static string PAUSE{ get{return "$pause";} }
        public static string CONTINUE{ get{return "$continue";} }
        public static string ABORT{ get{return "$abort";} }


        public string Signal;
        public string TaskUuid;

        public SigntalTaskCommand(string signal)
        {
            Command = "signal-task";
            ComUuid = Guid.NewGuid().ToString();

            this.Signal = signal;
            TaskUuid = Guid.NewGuid().ToString();
        }

    }

    public class StartTaskCommand : BaseCommand
    {
        public string ExecutionUnit;
        public Task Task;
        public string TaskUuid;

        public StartTaskCommand(Task task, string robot_name)
        {
            Command = "start-task";
            ComUuid = Guid.NewGuid().ToString();

            ExecutionUnit = robot_name;
            TaskUuid = Guid.NewGuid().ToString();
            Task = task;    
        }
        

    }

    // Defined but no docs on this, so ignoring for now.
    // public class QueryStatusCommand : Command
    // {

    // }

}