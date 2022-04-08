using System;
using SzyfryBlokowe.Enums;

namespace SzyfryBlokowe.Classes.Common
{
    public class AppManager
    {
        private readonly ConsoleInput _consoleInput = new();
        private readonly DataMapper _mapper = new();
        private AppLevel _appLevel;

        public AppManager(AppLevel appLevel)
        {
            _appLevel = appLevel;
        }

        public void Start()
        {
            PrintInfo();

            while (true)
            {
                var encryptType = MainMenu();

                if (encryptType is null)
                    return;

                Console.WriteLine();

                var numberFormat = NumberSystemMenu();

                if (numberFormat is null)
                    continue;

                Console.WriteLine();

                var result = StartGenerating((EncryptType)encryptType, (NumberFormat)numberFormat);

                PrintResult(result);
            }
        }

        private void PrintInfo()
        {
            Console.WriteLine("Encryptor application by Wiktor Szczeszek");
            Console.WriteLine();
        }      

        private EncryptType? MainMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Encript");
            Console.WriteLine("2. Decript");
            Console.WriteLine("3. Exit");

            var number = _consoleInput.InputNumber("Select menu option: ", 1, 3);

            Console.WriteLine();

            return number switch
            {
                1 => EncryptType.Encrypt,
                2 => EncryptType.Decrypt,
                _ => null,
            };
        }

        private NumberFormat? NumberSystemMenu()
        {
            Console.WriteLine("Number format");
            Console.WriteLine("1. Binary format");
            Console.WriteLine("2. Hexadecimal format");
            Console.WriteLine("3. Back");

            var number = _consoleInput.InputNumber("Select number system: ", 1, 3);

            Console.WriteLine();

            return number switch
            {
                1 => NumberFormat.Binary,
                2 => NumberFormat.Hexadecimal,
                _ => null,
            };
        }

        private BitMessage InputMessage(NumberFormat numberFormat)
        {
            if (numberFormat == NumberFormat.Binary)
                return _consoleInput.InputBitArray("Binary input:\t", 8);
            return _consoleInput.InputHexNumber("Hexadecimal input:\t", 2);
        }

        private BitMessage InputKey(NumberFormat numberFormat)
        {
            if (numberFormat == NumberFormat.Binary)
                return _consoleInput.InputBitArray("Binary key:\t", 8);
            return _consoleInput.InputHexNumber("Hexadecimal key:\t", 2);
        }

        private BitMessage StartGenerating(EncryptType encryptType, NumberFormat numberFormat)
        {
            var message = InputMessage(numberFormat);
            var key = InputKey(numberFormat);

            Console.WriteLine();

            var encryptor = new MessageEncryptor(message, key, _appLevel);

            return  encryptType == EncryptType.Encrypt ? encryptor.Encrypt(8) : encryptor.Decrypt(8);
        }

        private void PrintResult(BitMessage output)
        {
            Console.WriteLine();
            Console.WriteLine($"Result: (BIN){output} - (HEX){_mapper.BitMessageToHexNumber(output)}");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
