using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class ArrayExtensions
	{
        public static T[] CopyArray<T>(this T[] ts)
        {
			T[] _out = new T[ts.Length];
			ts.CopyTo(_out);
			return _out;
        }
		public static T[] CopyArray<T>(this T[] ts, int start = 0, int length = -1)
		{
			length = length == -1 ? ts.Length : length;

			return [.. ts.Skip(start).Take(length)];
		}

        public static int GetHashCodeEnumerable<T>(this T[] ts)
        {
            return GetHashCodeEnumerable<T>(ts as IEnumerable<T>);
        }
        public static int GetHashCodeEnumerable<T>(this IEnumerable<T> ts)
        {
            HashCode result = new();

            foreach (T t in ts) result.Add(t);

            return result.ToHashCode();
        }
    }
}
