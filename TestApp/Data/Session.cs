using System;
using System.Threading;

namespace MvcTest.Data
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
        public DateTime StartTime { get; set; }

        private static int SSID = 0;
        public static int NextID()
        {
            return Interlocked.Increment(ref SSID);
        }
    }
}