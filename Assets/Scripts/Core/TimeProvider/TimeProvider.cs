using System;

namespace Core.Time
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
        long TimestampUtcNow { get; }
    }
    
    public class TimeProvider: ITimeProvider
    {
        public long TimeOffset { get; set; }
        public DateTime UtcNow => DateTime.UtcNow;
        public long TimestampUtcNow => ToUnixTimeSeconds(UtcNow);
            
        private long ToUnixTimeSeconds(DateTime t) => ((DateTimeOffset)t).ToUnixTimeSeconds();
    }

}