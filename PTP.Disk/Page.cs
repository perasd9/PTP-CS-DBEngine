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

        /*
         * GetInt will convert byte value on specific offset from byte array to concrete int, it knows how much bytes because it's explcit ToInt32 which means 4B.
         */
        public int GetInt(int offset)
        {
            return BitConverter.ToInt32(_buffer, offset);
        }

        /*
         * SetInt would need to write int field(4B) into the page, calling system metod GetBytes(n) is gonna convert {n} value into bytes.
         * After that only copy value of that bytes into {_buffer} of page.
         */
        public void SetInt(int offset, int n)
        {
            var bytes = BitConverter.GetBytes(n);
            Array.Copy(bytes, 0, _buffer, offset, bytes.Length);
        }

        /*
         * GetBytes will navigate on offset(pos) in {_buffer}, from {_buffer} will be taken 4B(int) to get length of how much bytes is in that block
         * (ex. of disk block is: FIRST 4 BYTES = length of data, AFTER LENGTH(5. BYTE) is actual data), pos is increased for sizeof(int) to skip that LENGTH field.
         * {b} byte array is created as a actual data slot(after first int) and from {_buffer} reading {length} bytes and store in {b} and that's how bytes are gotten.
         */
        public byte[] GetBytes(int offset)
        {
            int pos = offset;
            int length = BitConverter.ToInt32(_buffer, pos);
            pos += sizeof(int);
            byte[] b = new byte[length];
            Array.Copy(_buffer, pos, b, 0, length);
            return b;
        }

        /*
         * SetBytes will navigate to offset(pos) in {b}, firstly {lengthBytes} will be gotten because as it's said structure of memory is first 4B(int) reserved for length.
         * Consenquently byte array {lengthBytes} will be written into {_buffer} and after that pos(offset) will be moved for sizeof(int) which is LENGTH and concrete content
         * will be written into {_buffer}
         */
        public void SetBytes(int offset, byte[] b)
        {
            int pos = offset;
            var lengthBytes = BitConverter.GetBytes(b.Length);
            Array.Copy(lengthBytes, 0, _buffer, pos, lengthBytes.Length);
            pos += sizeof(int);
            Array.Copy(b, 0, _buffer, pos, b.Length);
        }

        /*
         * GetBytes local method will bring byte array of concrete content into {_buffer} and from that {_buffer} value will be read with CHARSET.GetString method.
         */
        public string GetString(int offset)
        {
            byte[] b = GetBytes(offset);
            return CHARSET.GetString(b);
        }

        /*
         * {s} will be converted into byte array {b} and local method SetBytes will write that {b} into {_buffer} of page.
         */
        public void SetString(int offset, string s)
        {
            byte[] b = CHARSET.GetBytes(s);
            SetBytes(offset, b);
        }

        /*
         * CHARSET.GetMaxByteCount method will return number of bytes needed to store some string in specific Encoding type.
         * Consistently with structure of first 4B(int) this method will return maximum number of bytes needed to store our string in this system.
         */
        public static int MaxLength(int strlen)
        {
            float bytesPerChar = CHARSET.GetMaxByteCount(1);
            return sizeof(int) + (int)(strlen * bytesPerChar);
        }

        /*
         * Returns the content of this page as byte array {_buffer}
         */
        internal byte[] Contents()
        {
            return _buffer;
        }
    }
}
