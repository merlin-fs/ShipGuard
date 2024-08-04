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
        public struct WithDataTag : IComponentData {}
        public struct WaitSpawnTag : IComponentData {}
        public struct PostTag : IComponentData {}
        public struct DestroyTag : IComponentData {}
        public class Event : IComponentData
        {
            public Action<GameEntity> Callback;
        }
   }
}