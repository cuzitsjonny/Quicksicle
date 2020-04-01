using System;
using Quicksicle.Enums;

namespace Quicksicle.IO
{
    public class WriteOnlyBinaryLdf
    {
        private WriteOnlyBitStream binaryStream;
        private int count;

        public WriteOnlyBinaryLdf()
        {
            this.binaryStream = new WriteOnlyBitStream();
            this.count = 0;
        }

        public int Count
        {
            get { return count; }
        }

        public void AddWideString(string key, string value)
        {
            count++;

            binaryStream.Write((byte)(key.Length * 2));
            binaryStream.WriteWideString(key, key.Length);

            binaryStream.Write((byte)LdfDataType.WideString);
            binaryStream.Write(value.Length);
            binaryStream.WriteWideString(value, value.Length);
        }

        public void AddInt32(string key, int value)
        {
            count++;

            binaryStream.Write((byte)(key.Length * 2));
            binaryStream.WriteWideString(key, key.Length);

            binaryStream.Write((byte)LdfDataType.Int32);
            binaryStream.Write(value);
        }

        public void AddFloat(string key, float value)
        {
            count++;

            binaryStream.Write((byte)(key.Length * 2));
            binaryStream.WriteWideString(key, key.Length);

            binaryStream.Write((byte)LdfDataType.Float);
            binaryStream.Write(value);
        }

        public void AddBoolean(string key, bool value)
        {
            count++;

            binaryStream.Write((byte)(key.Length * 2));
            binaryStream.WriteWideString(key, key.Length);

            binaryStream.Write((byte)LdfDataType.Boolean);
            binaryStream.Write(value ? (byte)1 : (byte)0);
        }

        public void AddUInt64(string key, ulong value)
        {
            count++;

            binaryStream.Write((byte)(key.Length * 2));
            binaryStream.WriteWideString(key, key.Length);

            binaryStream.Write((byte)LdfDataType.UInt64);
            binaryStream.Write(value);
        }

        public void AddString(string key, string value)
        {
            count++;

            binaryStream.Write((byte)(key.Length * 2));
            binaryStream.WriteWideString(key, key.Length);

            binaryStream.Write((byte)LdfDataType.String);
            binaryStream.Write(value.Length);
            binaryStream.WriteString(value, value.Length);
        }

        // More data types later

        public byte[] ToByteArray()
        {
            return binaryStream.ToByteArray();
        }
    }
}
