using System;

namespace SzyfryBlokowe.Classes
{
    public class KeyGenerator
    {
        private BitMessage _keyInput, _keyLoop;
        private uint _loop = 0;
        private Func<BitMessage, BitMessage> _keyMarge = (a) 
            => new(new bool[] { a[0], a[2], a[4], a[6] });

        public readonly BitMessage InputedKey;

        public uint Loop => _loop;

        public BitMessage KeyLoop => _keyLoop;

        public KeyGenerator(BitMessage keyInput)
        {
            InputedKey = keyInput;
            _keyInput = keyInput;
        }

        public BitMessage GetNext()
        {
            if (++_loop % 2 != 0)
                SplitRotation();
            else
                MergeRotation();

            return GenerateKeyLoop();
        }

        private void MergeRotation()
            => _keyInput = _keyInput.RotateLeft(1);

        private void SplitRotation()
        {
            var dividedKey = _keyInput.DivideInHalf();
            _keyInput = new(dividedKey.Item1.RotateLeft(1), dividedKey.Item2.RotateLeft(1));
        }

        private BitMessage GenerateKeyLoop()
            => _keyLoop = _keyMarge(_keyInput);
    }
}
