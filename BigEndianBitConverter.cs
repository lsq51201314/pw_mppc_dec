namespace MppcDec
{
    public sealed class BigEndianBitConverter : EndianBitConverter
    {
        public override bool IsLittleEndian()
        {
            return false;
        }

        public override Endianness Endianness
        {
            get { return Endianness.BigEndian; }
        }

        protected override void CopyBytesImpl(long value, int bytes, byte[] buffer, int index)
        {
            var endOffset = index + bytes - 1;

            for (var i = 0; i < bytes; i++)
            {
                buffer[endOffset - i] = unchecked((byte)(value & 0xff));
                value = value >> 8;
            }
        }

        protected override long FromBytes(byte[] buffer, int startIndex, int bytesToConvert)
        {
            long ret = 0;

            for (var i = 0; i < bytesToConvert; i++)
            {
                ret = unchecked((ret << 8) | buffer[startIndex + i]);
            }

            return ret;
        }
    }
}
