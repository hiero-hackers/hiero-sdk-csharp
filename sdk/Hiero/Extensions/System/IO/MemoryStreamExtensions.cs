namespace System.IO
{
    public static class MemoryStreamExtensions
    {
        public static int ReadInt32(this MemoryStream ms)
        {
            var b = new byte[4];
            ms.Read(b, 0, 4);
            if (BitConverter.IsLittleEndian) Array.Reverse(b);
            return BitConverter.ToInt32(b);
        }
        public static long ReadInt64(this MemoryStream ms)
        {
            var b = new byte[8];
            ms.Read(b, 0, 8);
            if (BitConverter.IsLittleEndian) Array.Reverse(b);
            return BitConverter.ToInt64(b);
        }
    }
}
