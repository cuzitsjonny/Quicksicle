using System;

namespace Quicksicle.IO
{
    public class WriteOnlyBitStream
    {
        private static readonly byte[] ByteBitMasks = new byte[] { 128, 64, 32, 16, 8, 4, 2, 1 };

        private byte[] stream;
        private int bitsAllocated;
        private int bitsWritten;

        public WriteOnlyBitStream()
        {
            this.stream = new byte[64];
            this.bitsAllocated = 64 * 8;
            this.bitsWritten = 0;
        }

        public WriteOnlyBitStream(int bytesToBeAllocated)
        {
            this.stream = new byte[bytesToBeAllocated];
            this.bitsAllocated = bytesToBeAllocated * 8;
            this.bitsWritten = 0;
        }

        public int BitsWritten
        {
            get { return bitsWritten; }
        }

        private void CheckForReallocation(int bitsNeeded)
        {
            int bitsFree = bitsAllocated - bitsWritten;

            if (bitsFree < bitsNeeded)
            {
                AddBitsAndReallocate(bitsNeeded - bitsFree);
            }
        }

        private void AddBitsAndReallocate(int bitsToBeAdded)
        {
            int bytesToBeAdded = (int)Math.Ceiling(bitsToBeAdded / 8.0);
            byte[] reallocation = new byte[stream.Length + bytesToBeAdded];

            Array.Copy(stream, reallocation, stream.Length);

            stream = reallocation;
            bitsAllocated += bytesToBeAdded * 8;
        }

        public byte[] ToByteArray()
        {
            int bytesUsed = (int)Math.Ceiling(bitsWritten / 8.0);
            byte[] arr = new byte[bytesUsed];

            Array.Copy(stream, arr, bytesUsed);

            return arr;
        }

        public void Write(byte value)
        {
            CheckForReallocation(8);

            for (int i = 0; i < 8; i++)
            {
                int writeIndex = (int)Math.Floor(bitsWritten / 8.0);

                int readValue = value;
                int writeValue = stream[writeIndex];

                int readMask = ByteBitMasks[i];
                int writeMask = ByteBitMasks[bitsWritten % 8];

                if ((readValue & readMask) == readMask)
                {
                    writeValue = (writeValue | writeMask);
                    stream[writeIndex] = (byte)writeValue;
                }

                bitsWritten++;
            }
        }

        public void Write(byte[] value)
        {
            CheckForReallocation(value.Length * 8);

            for (int readIndex = 0; readIndex < value.Length; readIndex++)
            {
                for (int i = 0; i < 8; i++)
                {
                    int writeIndex = (int)Math.Floor(bitsWritten / 8.0);

                    int readValue = value[readIndex];
                    int writeValue = stream[writeIndex];

                    int readMask = ByteBitMasks[i];
                    int writeMask = ByteBitMasks[bitsWritten % 8];

                    if ((readValue & readMask) == readMask)
                    {
                        writeValue = (writeValue | writeMask);
                        stream[writeIndex] = (byte)writeValue;
                    }

                    bitsWritten++;
                }
            }
        }

        public void Write(sbyte value)
        {
            Write((byte)value);
        }

        public void Write(short value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(ushort value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(int value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(uint value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(ulong value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(bool value)
        {
            CheckForReallocation(1);

            if (value == true)
            {
                int writeIndex = (int)Math.Floor(bitsWritten / 8.0);
                int writeValue = stream[writeIndex];
                int writeMask = ByteBitMasks[bitsWritten % 8];

                writeValue = (writeValue | writeMask);
                stream[writeIndex] = (byte)writeValue;
            }

            bitsWritten++;
        }

        public void Write0()
        {
            Write(false);
        }

        public void Write1()
        {
            Write(true);
        }

        public void WriteString(string str, int length)
        {
            byte[] bytes = new byte[length];

            for (int i = 0; i < str.Length; i++)
            {
                if (i < length)
                {
                    bytes[i] = (byte)str[i];
                }
            }

            Write(bytes);
        }

        public void WriteWideString(string str, int length)
        {
            byte[] bytes = new byte[length * 2];

            for (int i = 0, b = 0; i < str.Length; i++, b += 2)
            {
                if (i < length)
                {
                    char character = str[i];
                    byte[] characterBytes = BitConverter.GetBytes(character);

                    bytes[b] = characterBytes[0];
                    bytes[b + 1] = characterBytes[1];
                }
            }

            Write(bytes);
        }
    }
}
