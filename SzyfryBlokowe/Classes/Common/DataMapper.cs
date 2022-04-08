using System;
using System.Linq;

namespace SzyfryBlokowe.Classes.Common
{
    public class DataMapper
    {
        private readonly char[] _hexChars = 
            { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F' };
        private readonly char[] _bitChars =
            { '0', '1' };

        public BitMessage BitArrayToBitMessage(string text) =>
            new(text.Select(c =>
                {
                    if (!_bitChars.Contains(c))
                        throw new ArgumentException("Text must contain numbers between 0 and 1.");

                    return c == '1';
                }).ToArray()
            );

        public BitMessage HexNumberToBitMessage(string text) =>
            new(text.SelectMany(c =>
            {
                if (!_hexChars.Contains(c))
                    throw new ArgumentException("Text must contain characters between 0 and F.");

                var converted = Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0');
                return BitArrayToBitMessage(converted).ToArray();
            }).ToArray());

        public string BitMessageToBitArray(BitMessage bitMessage) =>
            bitMessage.ToString();

        public string BitMessageToHexNumber(BitMessage bitMessage) =>
            Convert.ToInt32(BitMessageToBitArray(bitMessage), 2).ToString("X").PadLeft(2, '0');

        public int ConsoleKeyToIntNumber(ConsoleKeyInfo key, int min = int.MinValue, int max = int.MaxValue)
        {
            int number = int.Parse(key.KeyChar.ToString());

            if (number < min)
                throw new ArgumentException($"Specified number is to small, minimum number is {min}.");

            if (number > max)
                throw new ArgumentException($"Specified number is to big, maximum number is {max}.");

            return number;
        }
    }
}
