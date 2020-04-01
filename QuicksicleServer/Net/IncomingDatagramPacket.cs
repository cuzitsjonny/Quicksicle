using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksicle.Net
{
    public class IncomingDatagramPacket
    {
        public IncomingDatagramPacket(byte[] data, string sourceAddress, ushort sourcePort, string destinationAddress, ushort destinationPort)
        {
            this.Data = data;
            this.SourceAddress = sourceAddress;
            this.SourcePort = sourcePort;
            this.DestinationAddress = destinationAddress;
            this.DestinationPort = destinationPort;
        }

        public byte[] Data
        {
            get;
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
