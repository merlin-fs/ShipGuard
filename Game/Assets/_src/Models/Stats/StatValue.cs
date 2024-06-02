using System;

namespace Game.Model.Stats
{
    public interface IStatValue
    {
        StatValue Value { get; }
    }

    [Serializable]
    public struct StatValue
    {
        [Serializable]
        public struct ValueStruct
        {
            public float Min;
            public float Max;
            public float Value;
            public float Normalize;
        }

        public ValueStruct Original;
        public ValueStruct Current;
        #region extension
        public static implicit operator StatValue(float value)
        {
            return new StatValue()
            {
                Original = new ValueStruct()
                {
                    Min = 0,
                    Max = value,
                    Value = value,
                    Normalize = 1f,
                },

                Current = new ValueStruct()
                {
                    Min = 0,
                    Max = value,
                    Value = value,
                    Normalize = 1f,
                },
            };
        }

        public static StatValue Default => StatValueExt.Default;
        #endregion
    }

    public static class StatValueExt
    {
        public static readonly StatValue Default = 1f;
    }
}