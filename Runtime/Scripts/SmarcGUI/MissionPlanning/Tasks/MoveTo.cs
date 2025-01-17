using UnityEngine;
using System.Collections.Generic;
using System;

namespace SmarcGUI
{
    public class GeoPoint
    {
        public double latitude{get; set;}
        public double longitude{get; set;}
        public double altitude{get; set;}
    }

    public class MoveTo : Task
    {
        public static string SPEED_FAST{ get{return "fast";} }
        public static string SPEED_STANDARD{ get{return "standard";} }
        public static string SPEED_SLOW{ get{return "slow";} }


        public MoveTo(string description, string speed, GeoPoint waypoint)
        {
            Name = "move-to";
            Description = description;
            Params.Add("speed", speed);
            Params.Add("waypoint", waypoint);
        }

    }
}