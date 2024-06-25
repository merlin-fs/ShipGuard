using System;

using Common.Core;

using Unity.Entities;

using Game.Views;

using Unity.Mathematics;

namespace Game.Core.Spawns
{
    public partial struct Spawn : IComponentData, IStorageData
    {
        public Entity NewEntity;

        public struct ViewTag : IComponentData{}
        public struct ViewAttachTag : IComponentData {}
        public struct WithDataTag : IComponentData {}
        public struct PostSpawnTag : IComponentData {}

        public class Event : IComponentData
        {
            public Action<IView> Callback;
        }

   }
}