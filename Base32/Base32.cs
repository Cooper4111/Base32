using System;
using System.Collections.Generic;
using System.Text;

namespace Base32
{
    static class Base32
    {
        static string dict = "0123456789abcdefghijklmnopqrstuv"; // 5 bit 00000...11111
        static readonly byte[] base32masx = { // for general case BaseX mask can be generated runtime, but as the base value is known the constant array is cheaper
            0b_1111_1000,   // 0: 1          
            0b_0000_0111,   // 1: 2 - partial
            0b_1100_0000,   // 2: 2 - partial
            0b_0011_1110,   // 3: 3             
            0b_0000_0001,   // 4: 4 - partial   
            0b_1111_0000,   // 5: 4 - partial   
            0b_0000_1111,   // 6: 5 - partial   
            0b_1000_0000,   // 7: 5 - partial   
            0b_0111_1100,   // 8: 6             
            0b_0000_0011,   // 9: 7 - partial   
            0b_1110_0000,   // A: 7 - partial   
            0b_0001_1111,   // B: 8             
        };
        public static string EncodeBase32(string input)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(input);
            int rawLength = (strBytes.Length * 8 / 5);
            if (strBytes.Length * 8 % 5 != 0)
               rawLength++;
            char[] raw = new char[rawLength];
            int maskIndex = 0;
            int currStrByte = 0;
            int curBitPos = 0;
            int partA;
            int partB;
            for (int i = 0; i < rawLength; i++)
            {
                if (curBitPos == 8)
                    currStrByte++;
                curBitPos %= 8;

                if (curBitPos < 4)
                {
                    var charIndex = strBytes[currStrByte] & base32masx[maskIndex % 12]; // just apply mask
                    charIndex = charIndex >> 3 - curBitPos;
                    raw[i] = dict[charIndex];
                    maskIndex++;
                    curBitPos += 5;
                }
                else
                {
                    partA = strBytes[currStrByte] & base32masx[maskIndex % 12];
                    partA = partA << curBitPos - 3;
                    maskIndex++;
                    if (currStrByte + 1 < strBytes.Length)
                    {
                        
                        currStrByte++;
                        var strByte = strBytes[currStrByte];
                        var base32mask = base32masx[maskIndex % 12];
                        partB = strBytes[currStrByte] & base32masx[maskIndex % 12];
                        maskIndex++;
                        partB = partB >> 11 - curBitPos;
                        var charIndex = partB | partA;
                        raw[i] = dict[charIndex];
                        curBitPos += 5;
                    }
                    else
                    {
                        raw[i] = dict[partA];
                    }
                }
            }
            return new string(raw);
        }
        public static string DecodeBase32(string input)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(input);
            int rawLength = (strBytes.Length * 5 / 8);
            if (strBytes.Length % 8 != 0)
                rawLength++;
            byte[] output = new byte[rawLength];

            int curBitPos = 0;
            int currOutIndex = 0;
            for(int i = 0; i < input.Length; i++)
            {
                curBitPos %= 8;
                var value = input[i] > 57 ? input[i] - 87 : input[i] - 48;
                if (curBitPos < 4)
                {

                    output[currOutIndex] = (byte)(output[currOutIndex] | value << 3 - curBitPos);
                    curBitPos += 5;
                    if (curBitPos == 8)
                        currOutIndex++;
                }
                else
                {
                    output[currOutIndex] = (byte)(output[currOutIndex] | value >> curBitPos - 3);
                    curBitPos += 5;
                    currOutIndex++;
                    curBitPos %= 8;
                    output[currOutIndex] = (byte)(output[currOutIndex] | value << 8 - curBitPos);
                }
            }
            if(curBitPos != 8)
                Array.Resize(ref output, output.Length - 1);
            return Encoding.UTF8.GetString(output);
        }
    }
}
