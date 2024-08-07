//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Smarc
{
    [Serializable]
    public class FloatStampedMsg : Message
    {
        public const string k_RosMessageName = "smarc_msgs/FloatStamped";
        public override string RosMessageName => k_RosMessageName;

        public Std.HeaderMsg header;
        public float data;

        public FloatStampedMsg()
        {
            this.header = new Std.HeaderMsg();
            this.data = 0.0f;
        }

        public FloatStampedMsg(Std.HeaderMsg header, float data)
        {
            this.header = header;
            this.data = data;
        }

        public static FloatStampedMsg Deserialize(MessageDeserializer deserializer) => new FloatStampedMsg(deserializer);

        private FloatStampedMsg(MessageDeserializer deserializer)
        {
            this.header = Std.HeaderMsg.Deserialize(deserializer);
            deserializer.Read(out this.data);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.data);
        }

        public override string ToString()
        {
            return "FloatStampedMsg: " +
            "\nheader: " + header.ToString() +
            "\ndata: " + data.ToString();
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
