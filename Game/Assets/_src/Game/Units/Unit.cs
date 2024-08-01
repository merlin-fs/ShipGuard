﻿using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

using Common.Defs;

using Game.Core;
using Game.Core.Defs;
using Game.Core.Spawns;
using Game.Model.Stats;

using Unity.Transforms;

namespace Game.Model.Units
{
    [Serializable]
    public struct Unit : IUnit, IDefinable<Unit.Def>, IComponentData, IDefinableCallback
    {
        public RefLink<Def> RefLink { get; private set; }

        public void SetDef(ref RefLink<Def> link)
        {
            RefLink = link;
        }
        
        #region IDefineableCallback
        void IDefinableCallback.AddComponentData(Entity entity, IDefinableContext context)
        {
            context.AddComponentData(entity, this);
            context.AddComponentData(entity, new Move());
            context.AddComponentData(entity, new Spawn.ViewTag());
            context.AddComponentData(entity, new LocalTransform());
            context.AddComponentData(entity, new LocalToWorld());
        }
        #endregion
        
        [EnumHandle]
        public enum State
        {
            Stop,
            WeaponInRange,
        }

        #region Stats
        public struct Speed: IStat { }
        #endregion

        [Serializable]
        public class Def : IDef
        {
            [SerializeField, SelectType(typeof(IUnit))]
            private string storeType;
            public int GetTypeIndexDefinable() => ((ComponentType)Type.GetType(storeType)).TypeIndex;

            public List<ChildConfig> Parts = new();

            [SerializeReference, ReferenceSelect(typeof(IStatValue))]
            public IStatValue Speed;

            [SerializeReference, ReferenceSelect(typeof(IStatValue))]
            public IStatValue Health;
        }
    }
}