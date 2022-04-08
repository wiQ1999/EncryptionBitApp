using System;
using System.Collections;
using System.Text;

namespace SzyfryBlokowe.Classes
{
    public sealed class BitMessage : ICloneable, IEnumerable
    {
        private readonly bool[] _bitArray;

        public int Length => _bitArray.Length;

        public bool this[int index]
        {
            get => _bitArray[index];
            set => _bitArray[index] = value;
        }

        #region Constructors
        public BitMessage(int length)
        {
            _bitArray = new bool[length];
        }

        public BitMessage(int length, bool defaultValue) : this(length)
        {
            for (int i = 0; i < length; i++)
                _bitArray[i] = defaultValue;
        }

        public BitMessage(bool[] bitArray)
        {
            _bitArray = bitArray;
        }

        public BitMessage(BitMessage bitMessage)
        {
            _bitArray = bitMessage._bitArray;
        }

        public BitMessage(BitMessage bitMessage1, BitMessage bitMessage2)
        {
            _bitArray = new bool[bitMessage1.Length + bitMessage2.Length];
            Array.Copy(bitMessage1._bitArray, 0, _bitArray, 0, bitMessage1.Length);
            Array.Copy(bitMessage2._bitArray, 0, _bitArray, bitMessage1.Length, bitMessage2.Length);
        }
        #endregion

        #region Operators
        public static BitMessage operator &(BitMessage a, BitMessage b)
        {
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (a.Length != b.Length)
                throw new ArgumentException();

            BitMessage result = new(a.Length);

            for (int i = 0; i < result.Length; ++i)
                result[i] = a[i] & b[i];

            return result;
        }

        public static BitMessage operator |(BitMessage a, BitMessage b)
        {
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (a.Length != b.Length)
                throw new ArgumentException();

            BitMessage result = new(a.Length);

            for (int i = 0; i < result.Length; ++i)
                result[i] = a[i] | b[i];

            return result;
        }

        public static BitMessage operator ^(BitMessage a, BitMessage b)
        {
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (a.Length != b.Length)
                throw new ArgumentException();

            BitMessage result = new(a.Length);

            for (int i = 0; i < result.Length; ++i)
                result[i] = a[i] ^ b[i];

            return result;
        }

        public static BitMessage operator ~(BitMessage a)
        {
            BitMessage result = new(a.Length);

            for (int i = 0; i < result.Length; ++i)
                result[i] = !a[i];

            return result;
        }
        #endregion

        public BitMessage Copy(int startIndex, int endIndex)
        {
            if (startIndex > endIndex)
                throw new ArgumentException("Starting index is bigger than ending index.");

            if (startIndex < 0 || endIndex >= Length)
                throw new ArgumentOutOfRangeException();

            int count = endIndex - startIndex + 1;

            bool[] arrayToReturn = new bool[count];

            for (int i = 0; i < arrayToReturn.Length; i++)
                arrayToReturn[i] = this[startIndex + i];

            return new BitMessage(arrayToReturn);
        }

        public void Insert(BitMessage bitMessage, int index)
            => Insert(bitMessage._bitArray, index);

        public void Insert(bool[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("Array can not be null.");
            
            if (index < 0 || index + array.Length > Length)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < array.Length; i++)
                this[index + i] = array[i];
        }

        public (BitMessage, BitMessage) DivideInHalf()
        {
            int half = Length / 2;
            return (Copy(0, half - 1), Copy(half, Length - 1));
        }

        public BitMessage ShiftLeft(int count)
        {
            if (count < 0)
                throw new ArgumentException("Value cannot be less than 0.", nameof(count));

            if (count > Length)
                count = Length;

            var result = new bool[Length];

            Array.Copy(_bitArray, count, result, 0, Length - count);

            return new(result);
        }

        public BitMessage ShiftRight(int count)
        {
            if (count < 0)
                throw new ArgumentException("Value cannot be less than 0.", nameof(count));

            if (count > Length)
                count = Length;

            var result = new bool[Length];

            Array.Copy(_bitArray, 0, result, count, Length - count);

            return new(result);
        }

        public BitMessage RotateLeft(int count)
        {
            count %= Length;
            return ShiftLeft(count) | ShiftRight(Length - count);
        }

        public BitMessage RotateRight(int count)
        {
            count %= Length;
            return ShiftRight(count) | ShiftLeft(Length - count);
        }

        public object Clone()
            =>  new BitMessage(this);

        public IEnumerator GetEnumerator()
            => _bitArray.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool[] ToArray()
        {
            bool[] result = new bool[Length];
            Array.Copy(_bitArray, 0, result, 0, _bitArray.Length);
            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new(Length);

            foreach (bool bit in _bitArray)
                sb.Append(bit ? "1" : "0");

            return sb.ToString();
        }
    }
}
