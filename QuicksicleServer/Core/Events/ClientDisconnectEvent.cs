using System;

using Quicksicle.Events;

namespace Quicksicle.Core.Events
{
    public class ClientDisconnectEvent : BaseEvent
    {
        public ClientDisconnectEvent(string sourceAddress, ushort sourcePort, string destinationAddress, ushort destinationPort)
        {
            this.SourceAddress = sourceAddress;
            this.SourcePort = sourcePort;
            this.DestinationAddress = destinationAddress;
            this.DestinationPort = destinationPort;
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
    }
}
