using System;
using UnityEngine;

namespace Game.Model.Stats
{
    [Serializable]
    public class StaticValue: IStatValue
    {
        public float Value;
        StatValue IStatValue.Value => Value;
    }
}
