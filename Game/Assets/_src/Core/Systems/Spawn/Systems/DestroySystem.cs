using Common.Core;

using Game.Core.HybridTransforms;
using Game.Core.Repositories;
using Game.Model;

using Unity.Entities;

using Reflex.Attributes;

using Unity.Collections;

namespace Game.Core.Spawns
{
    public partial struct Spawn
    {
        [UpdateInGroup(typeof(GameSpawnSystemGroup))]
        public partial struct DestroySystem : ISystem
        {
            private static string m_PrefabType;
            
            [Inject] private static ConfigRepository m_Repository;
            [Inject] private static GameEntityRepository m_GameEntityRepository;
            
            private EntityQuery m_Query; 
            
            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<DestroyTag>()
                    .Build();
            }

            public void OnUpdate(ref SystemState state)
            {
                if (m_Query.IsEmpty) return;
                
                var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);

                //Destroy view
                foreach (var viewReference in SystemAPI.Query<HybridTransform.ViewReference>()
                             .WithAll<DestroyTag>())
                {
                    UnityEngine.Object.Destroy(viewReference.Value.Transform.gameObject);
                }

                //Remove entity
                foreach (var (gameEntity, entity) in SystemAPI.Query<GameEntity>()
                             .WithAll<DestroyTag>()
                             .WithEntityAccess())
                {
                    m_GameEntityRepository.Remove(gameEntity.ID);
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}