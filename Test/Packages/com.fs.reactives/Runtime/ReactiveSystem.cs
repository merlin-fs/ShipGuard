using Unity.Entities;

namespace Reactives
{
   
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct ReactiveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach(var changeEvents in SystemAPI.Query<DynamicBuffer<DataChangeEvent>>()
                        .WithAll<UserStorageDataTag, UserStorageDataChangeTag>()
                        .WithChangeFilter<UserStorageDataChangeTag>())
            {
                foreach (var @event in changeEvents)
                {
                    @event.Invoke();
                }
            }
        }
    }
}