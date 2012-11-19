/// Copyright 2006 Damien Guard
/// http://damieng.com/blog/2006/08/08/Calculating_CRC32_in_C_and_NET

using System;
using System.Text;

public static class Crc32
{
    public static int GetHashOfString(string str)
    {
        return GetHashOfBytes(Encoding.UTF8.GetBytes(str));
    }

    public static int GetHashOfBytes(byte[] data)
    {
        return (int)~CalculateHash(DefaultTable, DefaultSeed, data, 0, data.Length);
    }

    private const UInt32 DefaultPolynomial = 0xedb88320;
    private const UInt32 DefaultSeed = 0xffffffff;
    private static uint[] DefaultTable = InitializeTable(DefaultPolynomial);

    private static UInt32[] InitializeTable(UInt32 polynomial)
    {
        UInt32[] createTable = new UInt32[256];
        for (int i = 0; i < 256; i++)
        {
            UInt32 entry = (UInt32)i;
            for (int j = 0; j < 8; j++)
                if ((entry & 1) == 1)
                    entry = (entry >> 1) ^ polynomial;
                else
                    entry = entry >> 1;
            createTable[i] = entry;
        }

        return createTable;
    }

    private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, byte[] buffer, int start, int size)
    {
        UInt32 crc = seed;
        for (int i = start; i < size; i++)
            unchecked
            {
                crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
            }
        return crc;
    }
}