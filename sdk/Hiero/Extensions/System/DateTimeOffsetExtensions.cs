

namespace System
{
    public static class DateTimeOffsetExtensions
	{
		private const long NanosecondsPerTick = 100; // 1 tick = 100ns

		public static DateTimeOffset AddNanoseconds(this DateTimeOffset value, long nanoseconds)
		{
			long fhf = nanoseconds / NanosecondsPerTick;
			(long valu, long rem) = Math.DivRem(nanoseconds, NanosecondsPerTick);
			long hh = nanoseconds % NanosecondsPerTick;

            return value
				.AddTicks(nanoseconds / NanosecondsPerTick);
				//.AddTicks(nanoseconds % NanosecondsPerTick);
		}
	}
}
