using System;

namespace Quicksicle.IO
{
    public interface ISerializable
    {
        void ToBitStream(WriteOnlyBitStream packetStream);
    }
}
