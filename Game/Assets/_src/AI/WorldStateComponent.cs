using Unity.Entities;
using Unity.Properties;

using UnityEngine;

namespace Game.AI.GOAP
{
    public struct WorldStateComponent : IBufferElementData
    {
        [HideInInspector]
        public WorldStateHandle Value;

        [CreateProperty(ReadOnly = true)]
        public string DebugValue => Value.ToString();
    }
}
