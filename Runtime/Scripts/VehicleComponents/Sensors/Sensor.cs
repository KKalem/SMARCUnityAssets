using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils = DefaultNamespace.Utils;

namespace VehicleComponents.Sensors
{
    public interface ISensor
    {
        public float Frequency();
        public bool HasNewData();
    }

    public class Sensor: LinkAttachment, ISensor
    {
        [Header("Sensor")]
        public float frequency = 10f;
        public bool hasNewData = false;

        private float period => 1.0f/frequency;
        private double lastTime;


        public float Frequency()
        {
            return frequency;
        }

        public bool HasNewData()
        {
            return hasNewData;
        }


        double NowTimeInSeconds()
        {
            // copy from TF2/Clock.cs.
            // Why? Very little chance we'll implement a different thing
            // but i want keep this script free of ros-related things.
            double UnityUnscaledTimeSinceFrameStart = Time.realtimeSinceStartupAsDouble - Time.unscaledTimeAsDouble;
            return Time.timeAsDouble + UnityUnscaledTimeSinceFrameStart * Time.timeScale;
        }

        public virtual bool UpdateSensor(double deltaTime)
        {
            Debug.Log("This sensor needs to override UpdateSensor!");
            return false;
        }

        void FixedUpdate()
        {
            var deltaTime = NowTimeInSeconds() - lastTime;
            if(deltaTime < period) return;
            hasNewData = UpdateSensor(deltaTime);
            lastTime = NowTimeInSeconds();
        }

    }
}