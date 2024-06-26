//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Smarc
{
    [Serializable]
    public class LeakMsg : Message
    {
        public const string k_RosMessageName = "smarc_msgs/Leak";
        public override string RosMessageName => k_RosMessageName;

        public bool value;
        public int leak_counter;

        public LeakMsg()
        {
            this.value = false;
            this.leak_counter = 0;
        }

        public LeakMsg(bool value, int leak_counter)
        {
            this.value = value;
            this.leak_counter = leak_counter;
        }

        public static LeakMsg Deserialize(MessageDeserializer deserializer) => new LeakMsg(deserializer);

        private LeakMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.value);
            deserializer.Read(out this.leak_counter);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.value);
            serializer.Write(this.leak_counter);
        }

        public override string ToString()
        {
            return "LeakMsg: " +
            "\nvalue: " + value.ToString() +
            "\nleak_counter: " + leak_counter.ToString();
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
