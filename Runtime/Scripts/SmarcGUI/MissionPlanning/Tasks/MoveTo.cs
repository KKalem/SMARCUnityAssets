using UnityEngine;
using System.Collections.Generic;
using System;

namespace SmarcGUI
{
    public class GeoPointValue : IJsonSerializable
    {
        public double Latitude{get; set;}
        public double Longitude{get; set;}
        public double Altitude{get; set;}


        public string ToJson()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            fields["latitude"] = Latitude.ToString();
            fields["longitude"] = Longitude.ToString();
            fields["altitude"] = Altitude.ToString();
            return JsonUtility.ToJson(fields);
        }
    }

    public class MoveTo : Task, IJsonSerializable
    {
        public static string SPEED_FAST{ get{return "fast";} }
        public static string SPEED_STANDARD{ get{return "standard";} }
        public static string SPEED_SLOW{ get{return "slow";} }


        public MoveTo(string description, string speed, GeoPointValue waypoint)
        {
            Name = "move-to";
            Description = description;
            Params.Add(new TaskParam(){Name="speed", Value=speed});
            Params.Add(new TaskParam(){Name="waypoint", Value=waypoint});
        }

    }
}