using Game.AI.GOAP;

using Unity.Entities;
using Game.Core;

namespace Game.Model
{
    public partial struct Target : IComponentData, Logic.IStateData
    {
        public Entity Value;

        public struct Query: IComponentData
        {
            public float Radius;
            public uint SearchTeams;
        }
        
        public struct UseFinderTag: IComponentData{}

        public struct PossibleTargets: IBufferElementData
        {
            public Entity Value;
            
            public static implicit operator PossibleTargets(Entity entity)
            {
                PossibleTargets result = default;
                result.Value = entity;
                return result;
            }
        }

        [EnumHandle]
        public enum State
        {
            HasPossibleTargets,
            Found,
            Dead,
        }
    }
}
