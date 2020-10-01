using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CommonClassLibs
{
    public class MotherOfRawPackets
    {
        public MotherOfRawPackets(int List_ClientID)
        {
            _iListClientID = List_ClientID;
            _RawPacketsList = new Queue<RawPackets>();
            _Remainder = new byte[1024];

            _bytesRemaining = 0;
        }

        public int iListClientID { get { return _iListClientID; } }
        public int bytesRemaining { get { return _bytesRemaining; } set { _bytesRemaining = value; } }
        public byte[] Remainder { get { return _Remainder; } set { _Remainder = value; } }

        
        public void AddToList(byte[] data, int SizeOfChunk)
        {
            lock (_RawPacketsList)
                _RawPacketsList.Enqueue(new RawPackets(_iListClientID, data, SizeOfChunk));
        }
        public void ClearList()
        {
            lock (_RawPacketsList)
                _RawPacketsList.Clear();
        }

        public RawPackets GetTopItem
        {
            get
            {
                RawPackets rp;
                lock (_RawPacketsList)
                    rp = _RawPacketsList.Dequeue();
                return rp;
            }
        }

        public int GetItemCount
        {
            get { return _RawPacketsList.Count; }
        }

        public void TrimTheFat()
        {
        }
        private int _iListClientID;
        private Queue<RawPackets> _RawPacketsList;

        private int _bytesRemaining;
        private byte[] _Remainder;
    }

    public class RawPackets
    {
        public RawPackets(int iClientId, byte[] theChunk, int sizeofchunk)
        {
            _dataChunk = new byte[sizeofchunk];
            _dataChunk = theChunk; 
            _iClientId = iClientId;
            _iChunkLen = sizeofchunk;
        }

        public byte[] dataChunk { get { return _dataChunk; } }
        public int iClientId { get { return _iClientId; } }
        public int iChunkLen { get { return _iChunkLen; } }

        private byte[] _dataChunk;
        private int _iClientId;
        private int _iChunkLen;
    }

    public class FullPacket
    {
        public FullPacket(int iFromClient, byte[] thePacket)
        {
            _ThePacket = new byte[1024];
            _ThePacket = thePacket;
            _iFromClient = iFromClient;
        }

        public byte[] ThePacket { get { return _ThePacket; } set { _ThePacket = value; } }
        public int iFromClient { get { return _iFromClient; } set { _iFromClient = value; } }

        private byte[] _ThePacket;
        private int _iFromClient;
    }

}

[Flags]
public enum APPLICATION
{
    None = 0,
    ClientTypeA = 1,
    ClientTypeB = 2
}

public enum APPLEVEL
{
    None = 0,
    ClientLevel1 = 1,
    ClientLevel2 = 2
}

public enum PACKETTYPES
{
    TYPE_Ping = 1,
    TYPE_PingResponse = 2,
    TYPE_RequestCredentials = 3,
    TYPE_MyCredentials = 4,
    TYPE_Registered = 5,
    TYPE_HostExiting = 6,
    TYPE_ClientData = 7,
    TYPE_ClientDisconnecting = 8,
    TYPE_CredentialsUpdate = 9,
    TYPE_Close = 10,
    TYPE_Message = 11,
    TYPE_MessageReceived = 12
}

public enum PACKETTYPES_SUBMESSAGE
{
    SUBMSG_MessageStart,
    SUBMSG_MessageGuts,
    SUBMSG_MessageEnd
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PACKET_DATA
{
    public UInt16 Packet_Type;
    public UInt16 Packet_Size;
    public UInt16 Data_Type;
    public UInt16 maskTo;
    public UInt32 idTo;
    public UInt32 idFrom;
    public UInt16 nAppLevel;

    public UInt32 Data1;
    public UInt32 Data2;
    public UInt32 Data3;
    public UInt32 Data4;
    public UInt32 Data5;

    public Int32 Data6;
    public Int32 Data7;
    public Int32 Data8;
    public Int32 Data9;
    public Int32 Data10;

    public UInt32 Data11;
    public UInt32 Data12;
    public UInt32 Data13;
    public UInt32 Data14;
    public UInt32 Data15;

    public Int32 Data16;
    public Int32 Data17;
    public Int32 Data18;
    public Int32 Data19;
    public Int32 Data20; 

    public UInt32 Data21;
    public UInt32 Data22;
    public UInt32 Data23;
    public UInt32 Data24;
    public UInt32 Data25;

    public Int32 Data26;
    public Int32 Data27;
    public Int32 Data28;
    public Int32 Data29;
    public Int32 Data30;

    public Double DataDouble1;
    public Double DataDouble2;
    public Double DataDouble3;
    public Double DataDouble4;
    public Double DataDouble5;
    
    public Int64 DataLong1;
    public Int64 DataLong2;
    public Int64 DataLong3;
    public Int64 DataLong4;
    public UInt64 DataULong1;
    public UInt64 DataULong2;
    public UInt64 DataULong3;
    public UInt64 DataULong4;
    
    public Int64 DataTimeTick1;
    
    public Int64 DataTimeTick2;
    
    public Int64 DataTimeTick3;
    
    public Int64 DataTimeTick4;
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 300)]
    public Char[] szStringDataA = new Char[300];
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
    public Char[] szStringDataB = new Char[150];
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
    public Char[] szStringDataC = new Char[150];
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
    public Char[] szStringData150 = new Char[150];
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PACKET_BIGSTRING
{
    public UInt16 Packet_Type; 
    public UInt32 idTo;
    public UInt32 idFrom;
    public UInt32 StringLength;
    public UInt32 Extra;
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1006)]
    public Char[] sBigString = new Char[1006];
}


public static class PACKET_FUNCTIONS
{
    public static Byte[] StructureToByteArray(Object obj)
    {
        Int32 rawsize = Marshal.SizeOf(obj);
        IntPtr buffer = Marshal.AllocHGlobal(rawsize);
        Marshal.StructureToPtr(obj, buffer, false);
        Byte[] rawdatas = new Byte[rawsize];
        Marshal.Copy(buffer, rawdatas, 0, rawsize);
        Marshal.FreeHGlobal(buffer);
        return rawdatas;
    }

    public static Object ByteArrayToStructure(Byte[] rawdatas, Type anytype)
    {
        Int32 rawsize = Marshal.SizeOf(anytype);
        if (rawsize > rawdatas.Length)
            return null;

        IntPtr buffer = Marshal.AllocHGlobal(rawsize);
        Marshal.Copy(rawdatas, 0, buffer, rawsize);
        Object retobj = Marshal.PtrToStructure(buffer, anytype);
        Marshal.FreeHGlobal(buffer);
        return retobj;
    }
}

public static class EnumerationExtensions
{
    public static bool Has<T>(this System.Enum type, T value)
    {
        try
        {
            return (((int)(object)type & (int)(object)value) == (int)(object)value);
        }
        catch
        {
            return false;
        }
    }

    public static bool Is<T>(this System.Enum type, T value)
    {
        try
        {
            return (int)(object)type == (int)(object)value;
        }
        catch
        {
            return false;
        }
    }


    public static T Add<T>(this System.Enum type, T value)
    {
        try
        {
            return (T)(object)(((int)(object)type | (int)(object)value));
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                string.Format(
                    "Could not append value from enumerated type '{0}'.",
                    typeof(T).Name
                    ), ex);
        }
    }

    public static T Remove<T>(this System.Enum type, T value)
    {
        try
        {
            return (T)(object)(((int)(object)type & ~(int)(object)value));
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                string.Format(
                    "Could not remove value from enumerated type '{0}'.",
                    typeof(T).Name
                    ), ex);
        }
    }

}

