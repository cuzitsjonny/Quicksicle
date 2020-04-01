using System;

namespace Quicksicle.IO
{
    public interface IUnserializable
    {
        void FromBitStream(ReadOnlyBitStream packetStream);
    }
}
