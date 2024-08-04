using Game.Core.HybridTransforms;
using Game.Model.Locations;

using Unity.Entities;
using Unity.Transforms;

namespace Game.Core.Spawns
{
    [UpdateInGroup(typeof(GameSystemGroup))]
    public partial struct LocationPointSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var system = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = system.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (refLocation, entity) in SystemAPI.Query<ReferenceLocation>()
                         .WithAll<Spawn.PostTag>()
                         .WithNone<Spawn.WaitSpawnTag>()
                         .WithEntityAccess())
            {
                ecb.AddComponent(entity, new Parent{Value = refLocation.Entity});
            }
        }
    }
}
