using System;

namespace NodaTime
{
    public static class DurationExtensions
    {
        public static Duration Plus(this Duration duration, TimeSpan timespan)
        {
            return duration.Plus(Duration.FromTimeSpan(timespan));
        }
        public static Duration PlusSeconds(this Duration duration, int totalseconds)
        {
            return duration.Plus(Duration.FromSeconds(totalseconds));
        }
        public static Duration PlusSeconds(this Duration duration, double totalseconds)
        {
            return duration.Plus(Duration.FromSeconds(totalseconds));
        }
        public static Duration PlusSeconds(this Duration duration, long totalseconds)
        {
            return duration.Plus(Duration.FromSeconds(totalseconds));
        }

        public static Instant ToInstant(this Duration duration, Instant? from = null)
        {
            Instant result = from ?? SystemClock.Instance.GetCurrentInstant();
            
            return result.Plus(duration);
        }
	}
}
