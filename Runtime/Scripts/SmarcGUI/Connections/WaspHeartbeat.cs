using System;
using System.Collections;
using System.Collections.Generic;
using MQTTnet;
using Newtonsoft.Json;
using UnityEngine;


namespace SmarcGUI
{

    public enum WaspUnitType
    {
        air,
        ground,
        surface,
        subsurface
    }

    public enum WaspLevels
    {
        sensor,
        direct_execution,
        tst_execution,
        delegation
    }

    [JsonObject(NamingStrategyType = typeof(Newtonsoft.Json.Serialization.KebabCaseNamingStrategy))]
    public class WaspHeartbeatMsg
    {
        public string AgentType;
        public string AgentUuid;
        public string[] Levels;
        public string Name;
        public float Rate;
        public double Stamp;
        public string Type = "Heartbeat";

        public WaspHeartbeatMsg(string agentType, string agentUuid, string[] levels, string name, float rate)
        {
            AgentType = agentType;
            AgentUuid = agentUuid;
            Levels = levels;
            Name = name;
            Rate = rate;
        }

        

        public string ToJson()
        {
            Stamp = Time.time;
            return JsonConvert.SerializeObject(this);
        }
    }

    public class WaspHeartbeat : MonoBehaviour
    {
        public WaspUnitType UnitType;
        public string Context = "smarc";
        public string TopicBase => $"{Context}/unit/{AgentType}/simulation/{AgentName}/";

        MQTTClient mqttClient;
        WaspHeartbeatMsg msg;

        public float HeartbeatRate = 1.0f;
        public string AgentUUID{get; private set;}
        public string AgentType => UnitType.ToString();
        public string AgentName => transform.root.name;

        void Awake()
        {
            AgentUUID = Guid.NewGuid().ToString();
            mqttClient = FindFirstObjectByType<MQTTClient>();

            msg = new WaspHeartbeatMsg(
                agentType: AgentType,
                agentUuid: AgentUUID,
                levels: new string[]{WaspLevels.sensor.ToString(), WaspLevels.direct_execution.ToString(), WaspLevels.tst_execution.ToString()},
                name: AgentName,
                rate: HeartbeatRate);

            StartCoroutine(HeartbeatCoroutine());
        }

        IEnumerator HeartbeatCoroutine()
        {
            while (true)
            {
                mqttClient.Publish(TopicBase + "heartbeat", msg.ToJson());
                yield return new WaitForSeconds(HeartbeatRate);
            }
        }
    }
}