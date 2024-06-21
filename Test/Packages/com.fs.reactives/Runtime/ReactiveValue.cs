using System;
using System.Runtime.InteropServices;

using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace Reactives
{
    public interface IStorageData {}

    public struct UserStorageDataTag : IComponentData, IStorageData {}
    public struct UserStorageDataChangeTag : IComponentData, IStorageData {}

    public struct DataChangeEvent : IBufferElementData
    {
        private IntPtr m_IntPtr;
        public void Invoke()
        {
            Marshal.GetDelegateForFunctionPointer<Action>(m_IntPtr).Invoke();
        }

        public static DataChangeEvent From(Action action)
        {
            var @event = default(DataChangeEvent);
            GCHandle.Alloc(action);
            @event.m_IntPtr = Marshal.GetFunctionPointerForDelegate(action);
            return @event;
        }  
    }
    
    
    public struct LocationStorageDataTag : IComponentData, IStorageData {}
    
    public struct StoreModel : IComponentData, IStorageData
    {
        public int ID;
    }

    public struct StoreModelItem : IBufferElementData, IStorageData
    {
        public int id { get; set; }
    }

    public class ReactiveAttribute : Attribute{}

    public interface IReactive<T>
        where T : unmanaged, IComponentData
    {
        ref readonly T Value { get; }
        IDisposable Subscribe(Action reaction);
        IReactiveWritable<T> Writable();
        IReactiveWritable<T> Writable<TW>()
            where TW : IReactiveWritable<T>;
    }

    public interface IReactiveWritable<T> : IDisposable
        where T : unmanaged, IComponentData
    {
        ref T Value { get; }
    }

    public unsafe readonly struct ReactiveValue<T> : IReactive<T>, IReactiveWritable<T>
        where T : unmanaged, IComponentData
    {
        private readonly RefRW<T> m_Value;
        private readonly Entity m_Entity;
        
        public ReactiveValue(Entity entity)
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var typeIndex = TypeManager.GetTypeIndex<T>();
            m_Value = new RefRW<T>(manager.GetCheckedEntityDataAccess()->GetComponentDataRW_AsBytePointer(entity, typeIndex), AtomicSafetyHandle.Create());
            m_Entity = entity;
        }

        public void Dispose()
        {
            GetNewEcb()
                .SetComponent(m_Entity, default(UserStorageDataChangeTag));
        }
        
        public ref T Value => ref m_Value.ValueRW;
        
        public IDisposable Subscribe(Action reaction)
        {
            GetNewEcb()
                .AppendToBuffer(m_Entity, DataChangeEvent.From(reaction));
            return null;
        }

        public IReactiveWritable<T> Writable()
        {
            return this;
        }

        public IReactiveWritable<T> Writable<TW>()
            where TW : IReactiveWritable<T>
        {
            return Activator.CreateInstance<TW>();
        }
        

        ref readonly T IReactive<T>.Value => ref m_Value.ValueRO;

        private static EntityCommandBuffer GetNewEcb()
        {
            var cmd = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            var ecb = cmd.CreateCommandBuffer();
            return ecb;
        }
    }
}