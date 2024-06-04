using System;
using System.Reflection;

using Unity.Entities;

namespace Game.Core.Defs
{
    public interface IDefinableContext
    {
        DynamicBuffer<T> AddBuffer<T>(Entity entity) where T : unmanaged, IBufferElementData;
        void AppendToBuffer<T>(Entity entity, T data) where T : unmanaged, IBufferElementData;
        void AddComponentData<T>(Entity entity, T data) where T : unmanaged, IComponentData;
        void RemoveComponent<T>(Entity entity) where T : unmanaged, IComponentData;
        void SetComponentEnabled<T>(Entity entity, bool value) where T : unmanaged, IEnableableComponent;
        void SetName(Entity entity, string name);
        Entity CreateEntity();
    }
    
    public interface IDefinableCallback
    {
        void AddComponentData(Entity entity, IDefinableContext context);
        void RemoveComponentData(Entity entity, IDefinableContext context);
    }
}