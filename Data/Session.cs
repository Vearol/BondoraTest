using System;
using System.Threading;

namespace Data
{
    public class Session
    {
        public const string Key = "_SessionName";

        public static Session New()
        {
            return new Session()
            {
                ID = NextID(),
                StartTime = DateTime.UtcNow
            };
        }

        public int ID { get; set; }

        private DateTime StartTime { get; set; }
        private static int SSID = 0;

        private static int NextID()
        {
            return Interlocked.Increment(ref SSID);
        }
    }
}