using RosMessageTypes.Std;

namespace Drone.PSDK_ROS2
{
    public class PsdkByte : PsdkBase<UInt8Msg>
    {
        public byte level = 5;
        protected override void UpdateMessage()
        {
            ROSMsg.data = level;
        }
        
    }
}