using System;
using System.Collections.Generic;
using SzyfryBlokowe.Classes.Common;
using SzyfryBlokowe.Enums;

namespace SzyfryBlokowe.Classes
{
    public class MessageEncryptor
    {
        private readonly DataMapper _mapper = new();
        private AppLevel _appLevel = AppLevel.Prod;
        private BitMessage _leftMessage, _rightMessage;
        private KeyGenerator _keyGenerator;
        private Queue<BitMessage> _allKeys;

        private Func<BitMessage, BitMessage, bool> _functionS1 = (x, k) 
            => x[0] ^ x[0] & x[2] ^ x[1] & x[3] ^ x[1] & x[2] & x[3] ^ x[0] & x[1] & x[2] & x[3] ^ k[0];
        private Func<BitMessage, BitMessage, bool> _functionS2 = (x, k)
            => x[1] ^ x[0] & x[2] ^ x[0] & x[1] & x[3] ^ x[0] & x[2] & x[3] ^ x[0] & x[1] & x[2] & x[3] ^ k[1];
        private Func<BitMessage, BitMessage, bool> _functionS3 = (x, k)
            => true ^ x[2] ^ x[0] & x[3] ^ x[0] & x[1] & x[3] ^ x[0] & x[1] & x[2] & x[3] ^ k[2];
        private Func<BitMessage, BitMessage, bool> _functionS4 = (x, k)
            => true ^ x[0] & x[1] ^ x[2] & x[3] ^ x[0] & x[1] & x[3] ^ x[0] & x[2] & x[3] ^ x[0] & x[1] & x[2] & x[3] ^ k[3];

        public readonly BitMessage InputedMessage;

        public MessageEncryptor(BitMessage input, BitMessage key)
        {
            var dividedInput = input.DivideInHalf();
            _leftMessage = dividedInput.Item1;
            _rightMessage = dividedInput.Item2;
            _keyGenerator = new(key);
            InputedMessage = input;
        }

        public MessageEncryptor(BitMessage input, BitMessage key, AppLevel appLevel) : this(input, key)
        {
            _appLevel = appLevel;
        }

        public BitMessage Encrypt(int loops)
        {
            if (_appLevel == AppLevel.Dev)
                Console.WriteLine($"Started encrypting");

            var result = Process(loops, EncryptType.Encrypt);

            if (_appLevel == AppLevel.Dev)
                Console.WriteLine($"Encryption end");

            return result;
        }

        public BitMessage Decrypt(int loops)
        {
            if (_appLevel == AppLevel.Dev)
                Console.WriteLine($"Started decrypting");

            var result = Process(loops, EncryptType.Decrypt);

            if (_appLevel == AppLevel.Dev)
                Console.WriteLine($"Decryption end");

            return result;
        }

        private BitMessage Process(int loops, EncryptType type)
        {
            if (loops < 1)
                throw new ArgumentException("Number of loops is to small.", nameof(loops));

            if (_appLevel == AppLevel.Dev)
            {
                Console.WriteLine($"Input:\t(BIN){InputedMessage} - (HEX){_mapper.BitMessageToHexNumber(InputedMessage)}");
                Console.WriteLine($"Key:\t(BIN){_keyGenerator.InputedKey} - (HEX){_mapper.BitMessageToHexNumber(_keyGenerator.InputedKey)}");
            }

            GetAllKeys(loops, type);

            MainLoop(loops);

            return new BitMessage(_leftMessage, _rightMessage);
        }

        private void MainLoop(int loops)
        {
            for (int i = 1; i <= loops; i++)
            {
                _leftMessage ^= CalculateFunctionS();

                if (i != loops)
                    SwapHalfMessages();
            }
        }

        private BitMessage CalculateFunctionS()
        {
            var key = _allKeys.Dequeue();

            var calculation = new BitMessage(
                new bool[] {
                    _functionS1(_rightMessage, key),
                    _functionS2(_rightMessage, key),
                    _functionS3(_rightMessage, key),
                    _functionS4(_rightMessage, key)
                }
            );

            if (_appLevel == AppLevel.Dev)
                Console.WriteLine($"Function S result: {calculation}");

            return calculation;
        }

        private Queue<BitMessage> GetAllKeys(int loops, EncryptType type)
        {
            var list = new List<BitMessage>(loops);

            for (int i = 0; i < loops; i++)
            {
                list.Add(_keyGenerator.GetNext());

                if (_appLevel == AppLevel.Dev)
                    Console.WriteLine($"Key generator loop: {_keyGenerator.Loop}; Key: {list[i]}");
            }

            if (type == EncryptType.Decrypt)
            {
                list.Reverse();

                if (_appLevel == AppLevel.Dev)
                    Console.WriteLine("Key queue reversed");
            }

            return _allKeys = new Queue<BitMessage>(list);
        }

        private void SwapHalfMessages()
        {
            BitMessage temp = new(_leftMessage);
            _leftMessage = _rightMessage;
            _rightMessage = temp;
        }
    }
}
