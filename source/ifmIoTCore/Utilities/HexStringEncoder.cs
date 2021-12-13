namespace ifmIoTCore.Utilities
{
    using System;
    using System.Text;

    public static class HexStringEncoder
    {
        public static string ByteArrayToHexString(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException();

            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString == null) throw new ArgumentNullException();

            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        public static string StringToHexString(string str)
        {
            var bytes = Encoding.Default.GetBytes(str);
            return ByteArrayToHexString(bytes);
        }

        public static string HexStringToString(string hexString)
        {
            var bytes = HexStringToByteArray(hexString);
            return Encoding.Default.GetString(bytes);
        }
    }
}