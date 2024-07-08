using System;
using System.Reflection;

using Game.Views;

using Unity.Entities;

using UnityEngine.Scripting;

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
        public virtual void AddComponentData(Entity entity, IDefinableContext context){}
        public virtual void RemoveComponentData(Entity entity, IDefinableContext context){}
        public virtual void InitializationView(IView view, Entity entity){}
    }
}