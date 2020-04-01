using System;
using Quicksicle.Events;
using Quicksicle.IO;

namespace Quicksicle.Core.Events
{
    public class GamePacketReceiveEvent : BaseEvent
    {
        public GamePacketReceiveEvent(string sourceAddress, ushort sourcePort, string destinationAddress, ushort destinationPort, IUnserializable packet)
        {
            this.SourceAddress = sourceAddress;
            this.SourcePort = sourcePort;
            this.DestinationAddress = destinationAddress;
            this.DestinationPort = destinationPort;
            this.Packet = packet;
        }

        public string SourceAddress
        {
            get;
        }

        public ushort SourcePort
        {
            get;
        }

        public string DestinationAddress
        {
            get;
        }

        public ushort DestinationPort
        {
            get;
        }

        public IUnserializable Packet
        {
            get;
        }
    }
}
