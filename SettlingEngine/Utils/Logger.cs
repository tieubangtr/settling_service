namespace SettlingEngine.Utils
{
    public class Logger
    {
        public static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        public static readonly NLog.Logger logData = NLog.LogManager.GetLogger("logData");
    }
}
