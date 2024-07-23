using System;

namespace Game.Model.Stats
{
    [Serializable]
    public class RandomValue : IStatValue
    {
        public float Min;
        public float Max;
        StatValue IStatValue.Value => Common.Core.Dice.Range(Min, Max);
    }
}
