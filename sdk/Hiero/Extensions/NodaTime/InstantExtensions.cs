using System;

namespace NodaTime
{
    public static class InstantExtensions
    {
        public static Instant Plus(this Instant instant, TimeSpan timespan)
        {
            return instant.Plus(Duration.FromTimeSpan(timespan));
        }
        public static Instant PlusSeconds(this Instant instant, int totalseconds)
        {
            return instant.Plus(Duration.FromSeconds(totalseconds));
        }
        public static Instant PlusSeconds(this Instant instant, double totalseconds)
        {
            return instant.Plus(Duration.FromSeconds(totalseconds));
        }
        public static Instant PlusSeconds(this Instant instant, long totalseconds)
        {
            return instant.Plus(Duration.FromSeconds(totalseconds));
        }
	}
}
