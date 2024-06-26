//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Smarc
{
    [Serializable]
    public class StringPairMsg : Message
    {
        public const string k_RosMessageName = "smarc_msgs/StringPair";
        public override string RosMessageName => k_RosMessageName;

        public string first;
        public string second;

        public StringPairMsg()
        {
            this.first = "";
            this.second = "";
        }

        public StringPairMsg(string first, string second)
        {
            this.first = first;
            this.second = second;
        }

        public static StringPairMsg Deserialize(MessageDeserializer deserializer) => new StringPairMsg(deserializer);

        private StringPairMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.first);
            deserializer.Read(out this.second);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.first);
            serializer.Write(this.second);
        }

        public override string ToString()
        {
            return "StringPairMsg: " +
            "\nfirst: " + first.ToString() +
            "\nsecond: " + second.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
