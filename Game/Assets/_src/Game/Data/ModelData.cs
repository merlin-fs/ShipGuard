using System;
using System.Runtime.InteropServices;

using Unity.Entities;

namespace Game.Model.Datas
{
    public struct ModelDataTag<T> : IComponentData, IStorageData
        where T : IComponentData, IStorageData
    {
    }

    public struct ModelDataChangeTag<T> : IComponentData, IStorageData
        where T : IComponentData, IStorageData
    {
    }
    
    public struct ModelDataEvent : IBufferElementData, IStorageData
    {
        private IntPtr m_IntPtr;
        public void Invoke()
        {
            Marshal.GetDelegateForFunctionPointer<Action>(m_IntPtr).Invoke();
        }

        public static ModelDataEvent From(Action action)
        {
            var @event = default(ModelDataEvent);
            GCHandle.Alloc(action);
            @event.m_IntPtr = Marshal.GetFunctionPointerForDelegate(action);
            return @event;
        }  
    }
    
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [Unity.Entities.DisableAutoCreation]
    public partial struct ReactiveSystem<T> : ISystem
        where T : IComponentData, IStorageData
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach(var changeEvents in SystemAPI.Query<DynamicBuffer<ModelDataEvent>>()
                        .WithAll<ModelDataTag<T>, ModelDataChangeTag<T>>()
                        .WithChangeFilter<ModelDataChangeTag<T>>())
            {
                foreach (var @event in changeEvents)
                {
                    @event.Invoke();
                }
            }
        }
    }
}
