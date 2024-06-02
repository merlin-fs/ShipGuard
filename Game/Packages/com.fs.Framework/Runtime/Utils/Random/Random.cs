using System;

namespace Common.Core
{
    public static partial class Dice
    {
        private static System.Random m_Rnd = new System.Random();

        public static float Value
        {
            get {
                //return (float)(((double)MurmurHash.GetInt(Rand.seed, Rand.iterations++) - -2147483648.0) / 4294967295.0);
                return (float)m_Rnd.NextDouble();
            }
        }

        public static float Range(float min, float max)
        {
            return max <= min 
                ? min 
                : (Value * (max - min)) + min;
        }

        public static int Range(int min, int max)
        {
            return m_Rnd.Next(min, max);
        }
    }
}
