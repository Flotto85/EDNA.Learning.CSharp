using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EDNA.Learning.CSharp.Lesson2
{
    public class Plc : IPlc
    {
        private bool connected = false;
        private bool connecting = false;
        private bool disposed = false;
        private readonly ILogger logger;
        private readonly List<byte[]> areas = new List<byte[]>();
        private readonly List<WriteData> toWrite = new List<WriteData>();
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

        public void WriteData(int area, int startbyte, byte[] data)
        {
            if (area < 1 || area > areaCount)
                throw new ArgumentException("Area must be between 1 and 10");
            if (startbyte < 0 || startbyte > (areaLength - 1))
                throw new ArgumentException("Startbyte must be between 0 and 99");
            if (data.Length <= 0)
                throw new ArgumentException("ByteCount must not be negative or zero");
            if (startbyte + data.Length > areaLength)
                throw new ArgumentException("Startbyte plus byteCount may not exceed area size of 100 bytes");

            lock (this)
            {
                if (!connected)
                    throw new Exception("Not connected");
                toWrite.Add(new Lesson2.WriteData() { Area = area - 1, Startbyte = startbyte, Data = data });
            }
        }

        private void WriteLoop()
        {
            while (!disposed)
            {
                lock (this)
                {
                    MoveAxes();
                    DoHandshake();
                    UpdateTemperature();
                    CopyWriteData();               
                }
                if (!disposed)
                    Thread.Sleep(100);
            }
        }

        private void UpdateTemperature()
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

        private void CopyWriteData()
        {
            foreach(var wr in toWrite)
            {
                //logger.Log($"Writing Area {wr.Area + 1}");
                for (int i = 0; i < wr.Data.Length; i++)
                {
                    areas[wr.Area][wr.Startbyte + i] = wr.Data[i];
                }
            }
            toWrite.Clear();
        }

        private void DoHandshake()
        {
            if ((areas[5][4] & 0x1) == 1)
            {
                if ((areas[4][4] & 0x1) == 0)
                {
                    areas[4][4] = (byte)(areas[4][4] | 0x1);
                    logger.Log("Writing of Axes data enabled");
                }
            }
            else
            {
                if ((areas[4][4] & 0x1) == 1)
                {
                    areas[4][4] = (byte)(areas[4][4] & 0xFE);
                    logger.Log("Writing of Axes data disabled");
                }
            }
        }

        private Dictionary<string, short> axes = new Dictionary<string, short>() { { "X", 0 }, { "Y", 0 }, { "Z", 0 } };

        private void MoveAxes()
        {
            if ((areas[5][4] & 0x1) == 0 && (areas[4][4] & 0x1) == 0)
            {
                MoveAxisIfNeccessary("X", 5);
                MoveAxisIfNeccessary("Y", 7);
                MoveAxisIfNeccessary("Z", 9);
            }
        }

        private void MoveAxisIfNeccessary(string axisName, int startbyte)
        {
            var axisValue = BitConverter.ToInt16(areas[5], startbyte);
            if (axes[axisName] != axisValue)
            {
                axes[axisName] = axisValue;
                logger.Log($"Moved axis {axisName} to {axes[axisName]}");
            }
        }
    }
}
