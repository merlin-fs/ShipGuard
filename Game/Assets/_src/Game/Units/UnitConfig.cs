using System;
using System.Collections.Generic;

using Common.Core;

using Unity.Entities;
using UnityEngine;

using Game.Core.Defs;
using Game.Model.Stats;
using Game.Views;

namespace Game.Model.Units
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Configs/Unit")]
    public class UnitConfig: GameObjectConfig, IConfigContainer, IConfigStats
    {
        public Unit.Def Value = new();
        
        [SerializeField] private Uuid uuid = default;
        public Uuid UnitID => uuid;

        protected override void Configure(Entity prefab, EntityManager manager, IDefinableContext context)
        {
            base.Configure(prefab, manager, context);
            Value.AddComponentData(prefab, manager, context);
        }

        IEnumerable<ChildConfig> IConfigContainer.Childs => Value.Parts;

        void IConfigStats.Configure(DynamicBuffer<Stat> stats)
        {
            Stat.AddStat<Stat.Health>(stats, Value.Health.Value);
            Stat.AddStat<Unit.Speed>(stats, Value.Speed.Value);
        }

        public override void Configure(IView view, Entity entity, EntityManager manager)
        {
            Value.InitializationView(view, entity, manager);
        }

        public override ComponentTypeSet GetComponentTypeSet() => new ComponentTypeSet(ComponentType.FromTypeIndex(Value.GetTypeIndexDefinable()));

        #region UNITY_EDITOR
#if UNITY_EDITOR
        private void Reset()
        {
            uuid = default;
            GenerateUuid();
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            GenerateUuid();
        }

        private void GenerateUuid()
        {
            var prefix = TypeManager.GetType(Value.GetTypeIndexDefinable()).Name;
            uuid = uuid.GetUid(prefix, GetInstanceID(), out var change);
            if (change) UnityEditor.EditorUtility.SetDirty(this);
        }
        
        private void OnDestroy()
        {
            if(!Application.isPlaying) uuid.Remove();
        }
#endif
        #endregion
    }
}
