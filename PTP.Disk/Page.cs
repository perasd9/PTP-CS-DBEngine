using System.Text;

namespace PTP.Disk
{
    public class Page
    {
        private byte[] _buffer;
        public static readonly Encoding CHARSET = Encoding.ASCII;

        // Constructor for creating data buffers
        public Page(int blockSize)
        {
            _buffer = new byte[blockSize];
        }

        // Constructor for creating log pages
        public Page(byte[] b)
        {
            _buffer = b;
        }

        public int GetInt(int offset)
        {
            return BitConverter.ToInt32(_buffer, offset);
        }

        public void SetInt(int offset, int n)
        {
            var bytes = BitConverter.GetBytes(n);
            Array.Copy(bytes, 0, _buffer, offset, bytes.Length);
        }

        public byte[] GetBytes(int offset)
        {
            int pos = offset;
            int length = BitConverter.ToInt32(_buffer, pos);
            pos += sizeof(int);
            byte[] b = new byte[length];
            Array.Copy(_buffer, pos, b, 0, length);
            return b;
        }

        public void SetBytes(int offset, byte[] b)
        {
            int pos = offset;
            var lengthBytes = BitConverter.GetBytes(b.Length);
            Array.Copy(lengthBytes, 0, _buffer, pos, lengthBytes.Length);
            pos += sizeof(int);
            Array.Copy(b, 0, _buffer, pos, b.Length);
        }

        public string GetString(int offset)
        {
            byte[] b = GetBytes(offset);
            return CHARSET.GetString(b);
        }

        public void SetString(int offset, string s)
        {
            byte[] b = CHARSET.GetBytes(s);
            SetBytes(offset, b);
        }

        public static int MaxLength(int strlen)
        {
            float bytesPerChar = CHARSET.GetMaxByteCount(1);
            return sizeof(int) + (int)(strlen * bytesPerChar);
        }

        internal byte[] Contents()
        {
            return _buffer;
        }
    }
}
