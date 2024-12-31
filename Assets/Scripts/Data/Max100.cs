using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct Max100
    {
        [SerializeField] private byte value;

        public Max100(byte value)
        {
            this.value = (byte)Mathf.Clamp(value, 0, 100);
        }

        public Max100(int value)
        {
            this.value = (byte)Mathf.Clamp(value, 0, 100);
        }

        public static Max100 operator +(Max100 a, Max100 b)
        {
            return new Max100(a.value + b.value);
        }

        public static Max100 operator +(Max100 a, int b)
        {
            return new Max100(a.value + b);
        }

        public static Max100 operator -(Max100 a, Max100 b)
        {
            return new Max100(a.value - b.value);
        }

        public static Max100 operator -(Max100 a, int b)
        {
            return new Max100(a.value - b);
        }

        public static Max100 operator *(Max100 a, float b)
        {
            return new Max100(Mathf.CeilToInt(a.value * b));
        }

        public static implicit operator Max100(int value)
        {
            return new Max100(value);
        }

        public static implicit operator int(Max100 value)
        {
            return value.value;
        }
    }
}