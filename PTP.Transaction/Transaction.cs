using PTP.Disk;

namespace PTP.Transaction
{
    public class Transaction
    {
        internal void Pin(Block block)
        {
            throw new NotImplementedException();
        }

        internal void SetInt(Block block, int offset, int value, bool v)
        {
            throw new NotImplementedException();
        }

        internal void SetString(Block block, int offset, string value, bool v)
        {
            throw new NotImplementedException();
        }

        internal void Unpin(Block block)
        {
            throw new NotImplementedException();
        }
    }
}
