using System;

namespace SzyfryBlokowe.Classes.Common
{
    public class ConsoleInput
    {
        private readonly DataMapper _mapper = new();

        public int InputNumber(string frontText, int min = int.MinValue, int max = int.MaxValue) =>
            WhileInputKey(input => _mapper.ConsoleKeyToIntNumber(input, min, max), frontText);

        private int WhileInputKey(Func<ConsoleKeyInfo, int> converter, string frontText = "")
        {
            while (true)
            {
                try
                {
                    Console.Write(frontText);
                    ConsoleKeyInfo input = Console.ReadKey();
                    
                    return converter(input);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(Environment.NewLine + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Environment.NewLine + ex.Message);
                }
            }
        }

        public BitMessage InputBitArray(string frontText, int length) => 
            WhileInputText(input => _mapper.BitArrayToBitMessage(input), frontText, length);

        public BitMessage InputHexNumber(string frontText, int length) => 
            WhileInputText(input => _mapper.HexNumberToBitMessage(input), frontText, length);

        private BitMessage WhileInputText(Func<string, BitMessage> converter, string frontText = "", int length = 0)
        {
            while (true)
            {
                try
                {
                    Console.Write(frontText);
                    string input = Console.ReadLine();

                    if (length != 0 && input.Length != length)
                        throw new ArgumentException($"Incorrect number of characters, text length must be {length} characters.");

                    return converter(input);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
