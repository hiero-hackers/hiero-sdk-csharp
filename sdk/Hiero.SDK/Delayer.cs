using System;
using System.Threading.Tasks;

namespace Hiero.SDK
{
	/// <include file="Delayer.cs.xml" path='docs/member[@name="T:Delayer"]' />
	public class Delayer(ExecutorService executor)
    {
		private readonly ExecutorService _executor = executor;

        /// <include file="Delayer.cs.xml" path='docs/member[@name="M:Delayer.DelayAsync(System.NodaTime.Duration)"]' />
        public Task DelayAsync(NodaTime.Duration delay)
		{
			return _executor.Submit(async () =>
			{
				await Task.Delay(delay.ToTimeSpan()).ConfigureAwait(false);
			});
		}
		/// <include file="Delayer.cs.xml" path='docs/member[@name="M:Delayer.DelayAsync(System.NodaTime.Duration,System.Action)"]' />
		public Task DelayAsync(NodaTime.Duration delay, Action action)
		{
			return _executor.Submit(async () =>
			{
				await Task.Delay(delay.ToTimeSpan()).ConfigureAwait(false);

				action();
			});
		}
		/// <include file="Delayer.cs.xml" path='docs/member[@name="M:Delayer.DelayAsync``1(System.NodaTime.Duration,System.Func{``0})"]' />
		public Task<T> DelayAsync<T>(NodaTime.Duration delay, Func<T> func)
		{
            var tcs = new TaskCompletionSource<T>();
			_executor.Submit(async () =>
			{
				try
				{
					await Task.Delay(delay.ToTimeSpan()).ConfigureAwait(false);
					var result = func();
					tcs.SetResult(result);
				}
				catch (Exception e)
				{
					tcs.SetException(e);
				}
			});
			return tcs.Task;
		}
	}
}