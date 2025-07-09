using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crc16
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] input_data = new byte[] { 0x00, 0xFF, 0x03, 0x0A, 0x0B, 0x0C };
           // CRC16 crc16 = new CRC16();
            ushort crc16_result = CRC16.CalculateCRC16(input_data, 0 , (ushort) input_data.Length);

            Console.WriteLine("result: {0:X}", crc16_result);
            Console.ReadLine();
     

        }

        class CRC16
        {
            static bool crc_tab16_init = false;
            static ushort[] crc_tab16 = new ushort[256];
            static readonly ushort CRC_START_16 = 0x0000;
            static readonly ushort CRC_POLY_16 = 0xD175;
            /*
             * The function crc_16() calculates the 16 bits CRC16 in one pass for a byte
             * string of which the beginning has been passed to the function. The number of
             * bytes to check is also a parameter. The number of the bytes in the string is
             * limited by the constant SIZE_MAX.
             */
            public static ushort CalculateCRC16(byte[] input_str, ushort start, ushort length)
            {
                ushort crc;
                if (!crc_tab16_init) InitCRC16Table();

                crc = CRC_START_16;
                /* Make sure there are enough bytes to cover the start and length */
                if (input_str?.Length > 0 &&
                    input_str.Length >= (start + length))
                {
                    for (int i = start; i < (start + length); i++)
                    {
                        crc = (ushort)((crc >> 8) ^ crc_tab16[(crc ^ input_str[i]) & 0x00FF]);
                    }
                }
                return crc;
            }


            /*
             * The function update_crc_16() calculates a new CRC-16 value based on the
             * previous value of the CRC and the next byte of data to be checked.
             */
            public static ushort UpdateCRC16(ushort crc, byte c)
            {
                if (!crc_tab16_init) InitCRC16Table();
                return (ushort)((crc >> 8) ^ crc_tab16[(crc ^ c) & 0x00FF]);
            }

            /*
             * For optimal performance uses the CRC16 routine a lookup table with values
             * that can be used directly in the XOR arithmetic in the algorithm. This
             * lookup table is calculated by the init_crc16_tab() routine, the first time
             * the CRC function is called.
             */
            private static void InitCRC16Table()
            {
                ushort i;
                ushort j;
                ushort crc;
                ushort c;

                for (i = 0; i < 256; i++)
                {
                    crc = 0;
                    c = i;
                    for (j = 0; j < 8; j++)
                    {
                        if (((crc ^ c) & 0x0001) > 0)
                            crc = (ushort)((crc >> 1) ^ CRC_POLY_16);
                        else
                            crc = (ushort)(crc >> 1);

                        c = (ushort)(c >> 1);
                    }
                    crc_tab16[i] = crc;
                    //Console.WriteLine("{0} : {1:X} ", i, crc);
                }
                crc_tab16_init = true;
            }
        }

    }
}
