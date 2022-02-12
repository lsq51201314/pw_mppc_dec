using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MppcDec
{
    public class ByteEx
    {

        public static string ByteToHex(byte[] data)
        {
            if (data == null)
                return string.Empty;

            try
            {
                StringBuilder sb = new StringBuilder(data.Length * 3);
                foreach (byte b in data)
                {
                    sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                }
                return sb.ToString().ToUpper();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static byte[] HexToByte(string hexStr)
        {
            if (string.IsNullOrEmpty(hexStr))
                return null;

            try
            {
                byte[] byteArray = new byte[hexStr.Length / 2];
                for (int i = 0; i < byteArray.Length; i++)
                {
                    int j = Convert.ToInt32(hexStr.Substring(i * 2, 2), 16);
                    byteArray[i] = (byte)j;
                }
                return byteArray;
            }
            catch
            {
                return null;
            }
        }
    }
}
