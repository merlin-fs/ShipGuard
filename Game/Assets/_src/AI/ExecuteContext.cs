using System;

using Unity.Entities;

namespace Game.AI.GOAP
{
    public partial struct Logic
    {
        public readonly struct ExecuteContext
        {
            private readonly WorldStateSystem m_WorldStateSystem;
            private readonly EntityManager m_EntityManager;
            public EntityManager EntityManager => m_EntityManager;

            public ExecuteContext(ref SystemState state)
            {
                m_EntityManager = state.EntityManager;
                m_WorldStateSystem = state.EntityManager.World.GetOrCreateSystemManaged<WorldStateSystem>();
            }
            
            public void SetWorldState<T>(Entity entity, T worldState, bool value)
                where T : struct, IConvertible
            {
                SetWorldState(entity, WorldStateHandle.FromEnum(worldState, value));
            }

            public void SetWorldState(Entity entity, WorldStateHandle value)
            {
                m_WorldStateSystem.ChangeWorldState(entity, value);
            }
        }
    }
}
