using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmarcGUI.MissionPlanning.Tasks
{
    [JsonObject(NamingStrategyType = typeof(Newtonsoft.Json.Serialization.KebabCaseNamingStrategy))]
    public struct GeoPoint
    {
        public double latitude{get; set;}
        public double longitude{get; set;}
        public float altitude{get; set;}
        public readonly string rostype{ get{return "GeoPoint";} }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public GeoPoint(string json)
        {
            var gp = JsonConvert.DeserializeObject<GeoPoint>(json);
            latitude = gp.latitude;
            longitude = gp.longitude;
            altitude = gp.altitude;
        }
    }

    public struct MoveSpeed
    {
        public static string FAST{ get{return "fast";} }
        public static string STANDARD{ get{return "standard";} }
        public static string SLOW{ get{return "slow";} }
    }

    public class MoveTo : Task
    {
        public MoveTo()
        {
            new MoveTo("...", MoveSpeed.SLOW, new GeoPoint());
        }

        public MoveTo(string description, string speed, GeoPoint waypoint)
        {
            Name = "move-to";
            Description = description;
            Params.Add("speed", speed);
            Params.Add("waypoint", waypoint);
        }
    }

    public class MovePath : Task
    {
        public MovePath()
        {
            new MovePath("...", MoveSpeed.SLOW, new List<GeoPoint>());
        }
        public MovePath(string description, string speed, List<GeoPoint> waypoints)
        {
            Name = "move-path";
            Description = description;
            Params.Add("speed", speed);
            Params.Add("waypoints", waypoints);
        }
    }
}