using System;

using Common.Core;

using Game.Model;

using Unity.Entities;

using Game.Views;

using Unity.Mathematics;

namespace Game.Core.Spawns
{
    public partial struct Spawn : IComponentData, IStorageData
    {
        public Entity NewEntity;
        public bool DontCreate;
        
        public struct ViewTag : IComponentData{}
        public struct ViewAttachTag : IComponentData {}
        public struct WithDataTag : IComponentData {}
        
        public struct PostTag : IComponentData {}

        public class Event : IComponentData
        {
            public Action<GameEntity> Callback;
        }

        public class Condition : IComponentData
        {
            public Func<GameEntity, bool> Value;
        }
   }
}