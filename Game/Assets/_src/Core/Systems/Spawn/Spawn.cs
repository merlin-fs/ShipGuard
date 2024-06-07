using System;

using Common.Core;

using Unity.Entities;

using Common.Defs;

using Game.Views;

using Newtonsoft.Json.Linq;

namespace Game.Core.Spawns
{
    public partial class Spawn : IComponentData
    {
        public int ID;
        public ObjectID PrefabID;
        public JToken Data;
        
        public struct Tag : IComponentData{}
        
        public struct ViewTag : IComponentData{}

        public class Event : IComponentData
        {
            public Action<IView> Callback;
        }

        public struct Component : IBufferElementData
        {
            public ComponentType ComponentType;
            public static implicit operator Component(ComponentType componentType) =>
                new Component {ComponentType = componentType};
        }
   }
}