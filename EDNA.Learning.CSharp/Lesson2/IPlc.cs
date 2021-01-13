using System;
using System.Collections.Generic;
using System.Text;

namespace EDNA.Learning.CSharp.Lesson2
{
    public interface IPlc : IDisposable
    {
        /// <summary>
        /// Establishes a connection to the PLC
        /// </summary>
        void Connect();

        /// <summary>
        /// Closes the connection with the PLC
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Reads data from the plc
        /// </summary>
        /// <param name="area">The data area. Possible values are 1-10. An area is 100 bytes long</param>
        /// <param name="byteCount">Count of bytes to be read. Up to 20 are possible</param>
        /// <param name="startbyte">The byte of the area to start reading from</param>
        /// <returns>An array with the size of byteCount</returns>
        byte[] ReadData(int area, int startbyte, int byteCount);

        /// <summary>
        /// Write data to the plc
        /// </summary>
        /// <param name="area">The data area. Possible values are 1-10. An area is 100 bytes long</param>
        /// <param name="startByte">The byte of the area to start writing to</param>
        /// <param name="data">The data to write</param>
        void WriteData(int area, int startbyte, byte[] data);
    }
}
