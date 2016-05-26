using System;

namespace DnsShell
{
    internal class EndianBitConverter
    {
        internal static Byte[] ToByte(UInt16 Value, Boolean IsLittleEndian)
        {
            if (BitConverter.IsLittleEndian & IsLittleEndian)
            {
                return BitConverter.GetBytes(Value);
            }
            else if (BitConverter.IsLittleEndian)
            {
                Value = (UInt16)System.Net.IPAddress.HostToNetworkOrder((Int16)Value);
                return BitConverter.GetBytes(Value);
            }
            else
            {
                return BitConverter.GetBytes(Value);
            }

        }
        
        internal static Byte[] ToByte(UInt32 Value, Boolean IsLittleEndian)
        {
            if (BitConverter.IsLittleEndian & IsLittleEndian)
            {
                return BitConverter.GetBytes(Value);
            }
            else if (BitConverter.IsLittleEndian)
            {
                Value = (UInt32)System.Net.IPAddress.HostToNetworkOrder((Int32)Value);
                return BitConverter.GetBytes(Value);
            }
            else
            {
                return BitConverter.GetBytes(Value);
            }
        }
    }
}
