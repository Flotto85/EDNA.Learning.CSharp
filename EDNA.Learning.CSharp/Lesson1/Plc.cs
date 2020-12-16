using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EDNA.Learning.CSharp.Lesson1
{
    public class Plc : IPlc
    {
        private bool connected = false;
        private bool connecting = false;
        private bool disposed = false;
        private readonly ILogger logger;
        private readonly List<byte[]> areas = new List<byte[]>();
        private readonly Thread writeThread;

        private const int areaCount = 10;
        private const int areaLength = 100;

        public Plc(ILogger logger)
        {
            this.logger = new SafeLogger(logger);
            for (int i = 0; i < areaLength; i++)
            {
                areas.Add(new byte[areaLength]);
            }
            writeThread = new Thread(new ThreadStart(WriteLoop));
            writeThread.IsBackground = true;
            writeThread.Start();
        }

        public void Dispose()
        {
            if (disposed)
                return;
            Disconnect();
            disposed = true;
        }

        public void Connect()
        {
            if (connected)
            {
                logger.Log("Already connected.");
                return;
            }
            if (connecting)
            {
                logger.Log("Already connecting");
                return;
            }
            lock (this)
            {
                connecting = true;
                logger.Log("Connecting");
                Thread.Sleep(2000);
                connecting = false;
                connected = true;
                logger.Log("Connected");
            }
        }

        public void Disconnect()
        {
            if (!connected)
            {
                logger.Log("Not connected");
                return;
            }
            lock (this)
            {
                connected = false;
                logger.Log("Disconnected");
            }
        }

        public byte[] ReadData(int area, int startbyte, int byteCount)
        {
            if (area < 1 || area > areaCount)
                throw new ArgumentException("Area must be between 1 and 10");
            if (startbyte < 0 || startbyte > (areaLength - 1))
                throw new ArgumentException("Startbyte must be between 0 and 99");
            if (byteCount <= 0)
                throw new ArgumentException("ByteCount must not be negative or zero");
            if (startbyte + byteCount > areaLength)
                throw new ArgumentException("Startbyte plus byteCount may not exceed area size of 100 bytes");
            
            lock (this)
            {
                if (!connected)
                    throw new Exception("Not connected");
                Thread.Sleep(100);
                var bytes = areas[area - 1];
                byte[] readBytes = new byte[byteCount];
                Array.Copy(bytes, startbyte, readBytes, 0, byteCount);
                return readBytes;
            }
        }

        private void WriteLoop()
        {
            while (!disposed)
            {
                WriteBytes();
                if (!disposed)
                    Thread.Sleep(100);
            }
        }

        private void WriteBytes()
        {
            var rand = new Random();
            var d = rand.NextDouble();
            int multiplier = rand.NextDouble() > 0.5d ? -1 : 1;
            float temperature = 23.0f + (float)multiplier * (float)d;
            var bytesToWrite = BitConverter.GetBytes(temperature);
            for (int i = 0; i< 4; i++)
            {
                areas[3][12 + i] = bytesToWrite[i];
            }
            
        }
    }
}
