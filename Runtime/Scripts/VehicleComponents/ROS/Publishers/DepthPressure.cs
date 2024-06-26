using UnityEngine;
using RosMessageTypes.Sensor;
using Unity.Robotics.Core; //Clock

using SensorPressure = VehicleComponents.Sensors.DepthPressure;

namespace VehicleComponents.ROS.Publishers
{
    [RequireComponent(typeof(SensorPressure))]
    class DepthPressure: SensorPublisher<FluidPressureMsg, SensorPressure>
    { 
        void Start()
        {
            ROSMsg.header.frame_id = sensor.linkName;
        }

        public override void UpdateMessage()
        {
            ROSMsg.header.stamp = new TimeStamp(Clock.time);
            ROSMsg.fluid_pressure = sensor.pressure;
        }
    }
}