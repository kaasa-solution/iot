// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Runtime.InteropServices;

namespace System.Device.I2c
{
    internal class UnixI2cFileTransferBus : UnixI2cBus
    {
        public UnixI2cFileTransferBus(int busFileDescriptor, int busId)
            : base(busFileDescriptor, busId)
        {
        }

        protected override unsafe void WriteReadCore(ushort deviceAddress, byte* writeBuffer, byte* readBuffer, ushort writeBufferLength, ushort readBufferLength, bool write, bool read)
        {
            int result = Interop.ioctl(BusFileDescriptor, (uint)I2cSettings.I2C_SLAVE_FORCE, deviceAddress);
            if (result < 0)
            {
                throw new IOException($"Error {Marshal.GetLastWin32Error()} performing I2C data transfer.");
            }

            if (write)
            {
                if (writeBuffer == null && writeBufferLength != 0)
                {
                    throw new ArgumentNullException(nameof(writeBuffer));
                }

                result = Interop.write(BusFileDescriptor, new IntPtr(writeBuffer), writeBufferLength);
                if (result < 0)
                {
                    throw new IOException($"Error {Marshal.GetLastWin32Error()} performing I2C data transfer.");
                }
            }

            if (read)
            {
                if (readBuffer == null && readBufferLength != 0)
                {
                    throw new ArgumentNullException(nameof(readBuffer));
                }

                result = Interop.read(BusFileDescriptor, new IntPtr(readBuffer), readBufferLength);
                if (result < 0)
                {
                    throw new IOException($"Error {Marshal.GetLastWin32Error()} performing I2C data transfer.");
                }
            }
        }
    }
}
