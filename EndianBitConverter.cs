using System;

namespace MppcDec
{
    public abstract class EndianBitConverter
    {
        public abstract bool IsLittleEndian();
        public abstract Endianness Endianness { get; }

        private static readonly LittleEndianBitConverter little = new LittleEndianBitConverter();

        public static LittleEndianBitConverter Little
        {
            get { return little; }
        }

        private static readonly BigEndianBitConverter big = new BigEndianBitConverter();

        public static BigEndianBitConverter Big
        {
            get { return big; }
        }

        public long DoubleToInt64Bits(double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        public double Int64BitsToDouble(long value)
        {
            return BitConverter.Int64BitsToDouble(value);
        }

        public unsafe int SingleToInt32Bits(float value)
        {
            return *((int*)&value);
        }

        public unsafe float Int32BitsToSingle(int value)
        {
            return *((float*)&value);
        }

        public bool ToBoolean(byte[] value, int startIndex)
        {
            CheckByteArgument(value, startIndex, 1);
            return BitConverter.ToBoolean(value, startIndex);
        }

        public char ToChar(byte[] value, int startIndex)
        {
            return unchecked((char)(CheckedFromBytes(value, startIndex, 2)));
        }

        public double ToDouble(byte[] value, int startIndex)
        {
            return Int64BitsToDouble(ToInt64(value, startIndex));
        }

        public float ToSingle(byte[] value, int startIndex)
        {
            return Int32BitsToSingle(ToInt32(value, startIndex));
        }

        public short ToInt16(byte[] value, int startIndex)
        {
            return unchecked((short)(CheckedFromBytes(value, startIndex, 2)));
        }

        public int ToInt32(byte[] value, int startIndex)
        {
            return unchecked((int)(CheckedFromBytes(value, startIndex, 4)));
        }

        public long ToInt64(byte[] value, int startIndex)
        {
            return CheckedFromBytes(value, startIndex, 8);
        }

        public ushort ToUInt16(byte[] value, int startIndex)
        {
            return unchecked((ushort)(CheckedFromBytes(value, startIndex, 2)));
        }

        public uint ToUInt32(byte[] value, int startIndex)
        {
            return unchecked((uint)(CheckedFromBytes(value, startIndex, 4)));
        }

        public ulong ToUInt64(byte[] value, int startIndex)
        {
            return unchecked((ulong)(CheckedFromBytes(value, startIndex, 8)));
        }

        private static void CheckByteArgument(byte[] value, int startIndex, int bytesRequired)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (startIndex < 0 || startIndex > value.Length - bytesRequired)
                throw new ArgumentOutOfRangeException("startIndex");
        }

        private long CheckedFromBytes(byte[] value, int startIndex, int bytesToConvert)
        {
            CheckByteArgument(value, startIndex, bytesToConvert);
            return FromBytes(value, startIndex, bytesToConvert);
        }

        protected abstract long FromBytes(byte[] value, int startIndex, int bytesToConvert);

        public static string ToString(byte[] value)
        {
            return BitConverter.ToString(value);
        }

        public static string ToString(byte[] value, int startIndex)
        {
            return BitConverter.ToString(value, startIndex);
        }

        public static string ToString(byte[] value, int startIndex, int length)
        {
            return BitConverter.ToString(value, startIndex, length);
        }

        public decimal ToDecimal(byte[] value, int startIndex)
        {
            var parts = new int[4];
            for (int i = 0; i < 4; i++)
                parts[i] = ToInt32(value, startIndex + i * 4);
            return new Decimal(parts);
        }

        public byte[] GetBytes(decimal value)
        {
            var bytes = new byte[16];
            var parts = decimal.GetBits(value);
            for (int i = 0; i < 4; i++)
                CopyBytesImpl(parts[i], 4, bytes, i * 4);
            return bytes;
        }

        public void CopyBytes(decimal value, byte[] buffer, int index)
        {
            var parts = decimal.GetBits(value);
            for (int i = 0; i < 4; i++)
                CopyBytesImpl(parts[i], 4, buffer, i * 4 + index);
        }

        private byte[] GetBytes(long value, int bytes)
        {
            var buffer = new byte[bytes];
            CopyBytes(value, bytes, buffer, 0);
            return buffer;
        }

        public byte[] GetBytes(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        public byte[] GetBytes(char value)
        {
            return GetBytes(value, 2);
        }

        public byte[] GetBytes(double value)
        {
            return GetBytes(DoubleToInt64Bits(value), 8);
        }

        public byte[] GetBytes(short value)
        {
            return GetBytes(value, 2);
        }

        public byte[] GetBytes(int value)
        {
            return GetBytes(value, 4);
        }

        public byte[] GetBytes(long value)
        {
            return GetBytes(value, 8);
        }

        public byte[] GetBytes(float value)
        {
            return GetBytes(SingleToInt32Bits(value), 4);
        }

        public byte[] GetBytes(ushort value)
        {
            return GetBytes(value, 2);
        }

        public byte[] GetBytes(uint value)
        {
            return GetBytes(value, 4);
        }

        public byte[] GetBytes(ulong value)
        {
            return GetBytes(unchecked((long)value), 8);
        }

        public byte[] GetCompactUInt32Bytes(int value)
        {
            return GetCompactUInt32Bytes((uint)value);
        }

        public byte[] GetCompactUInt32Bytes(uint value)
        {
            if (value < 0x80)
                return new byte[] { (byte)value };

            if (value < 0x4000)
                return Big.GetBytes((ushort)(value | 0x8000));

            if (value < 0x20000000)
                return Big.GetBytes((int)(value | 0xC0000000));

            var res = new byte[5];
            res[0] = 0xE0;

            Buffer.BlockCopy(Big.GetBytes(value), 0, res, 1, 4);

            return res;
        }

        private void CopyBytes(long value, int bytes, byte[] buffer, int index)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "Byte array must not be null");
            if (buffer.Length < index + bytes)
                throw new ArgumentOutOfRangeException("Buffer not big enough for value");
            CopyBytesImpl(value, bytes, buffer, index);
        }

        protected abstract void CopyBytesImpl(long value, int bytes, byte[] buffer, int index);

        public void CopyBytes(bool value, byte[] buffer, int index)
        {
            CopyBytes(value ? 1 : 0, 1, buffer, index);
        }

        public void CopyBytes(char value, byte[] buffer, int index)
        {
            CopyBytes(value, 2, buffer, index);
        }

        public void CopyBytes(double value, byte[] buffer, int index)
        {
            CopyBytes(DoubleToInt64Bits(value), 8, buffer, index);
        }

        public void CopyBytes(short value, byte[] buffer, int index)
        {
            CopyBytes(value, 2, buffer, index);
        }

        public void CopyBytes(int value, byte[] buffer, int index)
        {
            CopyBytes(value, 4, buffer, index);
        }

        public void CopyBytes(long value, byte[] buffer, int index)
        {
            CopyBytes(value, 8, buffer, index);
        }

        public void CopyBytes(float value, byte[] buffer, int index)
        {
            CopyBytes(SingleToInt32Bits(value), 4, buffer, index);
        }

        public void CopyBytes(ushort value, byte[] buffer, int index)
        {
            CopyBytes(value, 2, buffer, index);
        }

        public void CopyBytes(uint value, byte[] buffer, int index)
        {
            CopyBytes(value, 4, buffer, index);
        }

        public void CopyBytes(ulong value, byte[] buffer, int index)
        {
            CopyBytes(unchecked((long)value), 8, buffer, index);
        }
    }
}
