using System;
using System.IO;

namespace Quicksicle.IO
{
    public class ReadOnlyBitStream
    {
        private static readonly byte[] ByteBitMasks = new byte[] { 128, 64, 32, 16, 8, 4, 2, 1 };

        private byte[] stream;
        private int bitsAllocated;
        private int bitsRead;

        public ReadOnlyBitStream(byte[] byteArray)
        {
            this.stream = byteArray;
            this.bitsAllocated = byteArray.Length * 8;
            this.bitsRead = 0;
        }

        public void CheckForEndOfStream(int bitsNeeded)
        {
            int bitsUnread = bitsAllocated - bitsRead;

            if (bitsUnread < bitsNeeded)
            {
                throw new EndOfStreamException("Tried to read " + bitsNeeded + " bits, but only " + bitsUnread + " unread bits left.");
            }
        }

        public int BitsRead
        {
            get { return bitsRead; }
        }

        public int BitsUnread
        {
            get { return bitsAllocated - bitsRead; }
        }

        public void SkipBits(int bitsToBeSkipped)
        {
            CheckForEndOfStream(bitsToBeSkipped);

            bitsRead += bitsToBeSkipped;
        }

        public void SkipBytes(int bytesToBeSkipped)
        {
            CheckForEndOfStream(bytesToBeSkipped * 8);

            bitsRead += bytesToBeSkipped * 8;
        }

        public byte ReadByte()
        {
            byte value = 0;

            for (int i = 0; i < 8; i++)
            {
                int readIndex = (int)Math.Floor(bitsRead / 8.0);

                int readValue = stream[readIndex];
                int writeValue = value;

                int readMask = ByteBitMasks[bitsRead % 8];
                int writeMask = ByteBitMasks[i];

                if ((readValue & readMask) == readMask)
                {
                    writeValue = (writeValue | writeMask);
                    value = (byte)writeValue;
                }

                bitsRead++;
            }

            return value;
        }

        public byte[] ReadBytes(int length)
        {
            CheckForEndOfStream(length * 8);

            byte[] value = new byte[length];

            for (int writeIndex = 0; writeIndex < value.Length; writeIndex++)
            {
                for (int i = 0; i < 8; i++)
                {
                    int readIndex = (int)Math.Floor(bitsRead / 8.0);

                    int readValue = stream[readIndex];
                    int writeValue = value[writeIndex];

                    int readMask = ByteBitMasks[bitsRead % 8];
                    int writeMask = ByteBitMasks[i];

                    if ((readValue & readMask) == readMask)
                    {
                        writeValue = (writeValue | writeMask);
                        value[writeIndex] = (byte)writeValue;
                    }

                    bitsRead++;
                }
            }

            return value;
        }

        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        public short ReadInt16()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        public ushort ReadUInt16()
        {
            return BitConverter.ToUInt16(ReadBytes(2), 0);
        }

        public int ReadInt32()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        public uint ReadUInt32()
        {
            return BitConverter.ToUInt32(ReadBytes(4), 0);
        }

        public long ReadInt64()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }

        public ulong ReadUInt64()
        {
            return BitConverter.ToUInt64(ReadBytes(8), 0);
        }

        public float ReadSingle()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        public bool ReadBit()
        {
            CheckForEndOfStream(1);

            bool value = false;

            int readIndex = (int)Math.Floor(bitsRead / 8.0);
            int readValue = stream[readIndex];
            int readMask = ByteBitMasks[bitsRead % 8];

            if ((readValue & readMask) == readMask)
            {
                value = true;
            }

            bitsRead++;

            return value;
        }

        public string ReadString(int length)
        {
            string str = String.Empty;
            byte[] bytes = ReadBytes(length);
            bool reachedNullTerminator = false;

            for (int i = 0; i < length && !reachedNullTerminator; i++)
            {
                if (bytes[i] == 0)
                {
                    reachedNullTerminator = true;
                }
                else
                {
                    str += (char)bytes[i];
                }
            }

            return str;
        }

        public string ReadWideString(int length)
        {
            string str = String.Empty;
            int byteLength = length * 2;
            byte[] bytes = ReadBytes(byteLength);
            bool reachedNullTerminator = false;

            for (int i = 0; i < byteLength && !reachedNullTerminator; i += 2)
            {
                char character = BitConverter.ToChar(bytes, i);

                if (character == 0)
                {
                    reachedNullTerminator = true;
                }
                else
                {
                    str += character;
                }
            }

            return str;
        }
    }
}
