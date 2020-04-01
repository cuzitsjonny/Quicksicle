using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksicle.Net
{
    public class OutgoingDatagramPacket
    {
        public OutgoingDatagramPacket(byte[] data,string destinationAddress, ushort destinationPort)
        {
            this.Data = data;
            this.DestinationAddress = destinationAddress;
            this.DestinationPort = destinationPort;
        }

        public byte[] Data
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
