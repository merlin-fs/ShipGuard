namespace Common.Core
{
    public static partial class Dice
    {
        public static float Roll(int N, int S)
        {
            int value = 0;
            for (int i = 0; i < N; i++)
                value += 1 + Range(0, S);
            return value;
        }
        /// <summary>
        /// Выбор куда попали 
        /// </summary>
        /// <param name="N">Количество частей</param>
        /// <param name="Idx">Приоритетная часть (например, прицельный выстрел в голову: RollBody(6, 1))</param>
        /// <returns>Часть в которую попали</returns>
        public static float RollPart(int N, int Idx = 0)
        {
            int[] map = new int[N];
            if (Idx > 0)
                map[Idx-1] = 3;
            int max = 0;
            int n = Idx-1;
            for (int i = 0; i < N*2; i++)
            {
                int tmp = Range(0, N);
                map[tmp]++;
                if (max < map[tmp])
                {
                    n = tmp;
                    max = map[tmp];
                }
            }
            return n + 1;
        }
        public static float RollDropMin(float N, float S)
        {
            float value = 0;
            float minValue = N * S;
            for (int i = 0; i < N; i++)
            {
                float tmp = 1 + Range(0, S);
                minValue = System.Math.Min(tmp, minValue);
                value += tmp;
            }
            value -= minValue;
            return value;
        }

        public static float RollStat(float min, float max)
        {
            return RollDropMin((min + 1), max / (min + 1));
        }
    }
}
