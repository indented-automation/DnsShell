using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DnsShell
{
    // DnsShell.IO.EndianBinaryReader - A modified version of System.IO.BinaryReader 
    // allowing for control over Endian order for certain methods.
    //
    // Constructors:
    //   Public:  EndianBinaryReader(Stream BaseStream)
    //
    // Methods:
    //   Public Override: UInt16()
    //   Public Override: UInt32()

    internal class EndianBinaryReader : BinaryReader
    {
        internal EndianBinaryReader(Stream BaseStream) : base(BaseStream) { }

        internal UInt16 ReadUInt16(Boolean IsBigEndian)
        {
            return (UInt16)((base.ReadByte() << 8) | base.ReadByte());
        }

        internal UInt32 ReadUInt32(Boolean IsBigEndian)
        {
            return (UInt32)(
                (base.ReadByte() << 24) |
                (base.ReadByte() << 16) |
                (base.ReadByte() << 8) |
                base.ReadByte());
        }

        internal UInt64 ReadUInt64(Boolean IsBigEndian)
        {
            return (UInt64)(
                (base.ReadByte() << 56) |
                (base.ReadByte() << 48) |
                (base.ReadByte() << 40) |
                (base.ReadByte() << 32) |
                (base.ReadByte() << 24) |
                (base.ReadByte() << 16) |
                (base.ReadByte() << 8) |
                base.ReadByte());
        }

        internal Byte PeekByte()
        {
            Byte Value = base.ReadByte();
            base.BaseStream.Seek(-1, System.IO.SeekOrigin.Current);
            return Value;
        }

        internal IPAddress ReadIPAddress()
        {
            return IPAddress.Parse(
                String.Format("{0}.{1}.{2}.{3}",
                base.ReadByte(),
                base.ReadByte(),
                base.ReadByte(),
                base.ReadByte()));
        }

        internal IPAddress ReadIPv6Address()
        {
            return IPAddress.Parse(
                String.Format("{0:X}:{1:X}:{2:X}:{3:X}:{4:X}:{5:X}:{6:X}:{7:X}",
                    this.ReadUInt16(true),
                    this.ReadUInt16(true),
                    this.ReadUInt16(true),
                    this.ReadUInt16(true),
                    this.ReadUInt16(true),
                    this.ReadUInt16(true),
                    this.ReadUInt16(true),
                    this.ReadUInt16(true)));
        }
    }
}
