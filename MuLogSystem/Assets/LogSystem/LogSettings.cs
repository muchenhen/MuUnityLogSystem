using UnityEngine;

namespace LogSystem
{
    [CreateAssetMenu(fileName = "LogSettings", menuName = "Settings/LogSettings")]
    public class LogSettings : ScriptableObject
    {
        public Logger.LogLevel logOutputLevel = Logger.LogLevel.Display;
        
        public bool openShippingLog = false;
    }
}