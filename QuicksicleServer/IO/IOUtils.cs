using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksicle.IO
{
    public static class IOUtils
    {
        public static readonly int NibbleBitMask = 15; // 00001111
        public static readonly char[] HexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        public static string ConvertBin2Hex(byte[] bin)
        {
            string hex = String.Empty;

            for (int i = 0; i < bin.Length; i++)
            {
                hex += HexDigits[(bin[i] & (NibbleBitMask << 4)) >> 4];
                hex += HexDigits[bin[i] & NibbleBitMask];
            }

            return hex;
        }

        public static byte[] ConvertHex2Bin(string hex)
        {
            byte[] bin = new byte[hex.Length / 2];

            for (int i = 0, b = 0; b < hex.Length; i++, b += 2)
            {
                bin[i] = Convert.ToByte(hex.Substring(b, 2), 16);
            }

            return bin;
        }
    }
}
