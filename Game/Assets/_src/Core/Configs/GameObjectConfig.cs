using System;

using Game.Core.Spawns;
using Game.Model;

using Unity.Entities;

using Game.Model.Stats;

using Unity.Transforms;

namespace Game.Core.Defs
{
    public abstract class GameObjectConfig : ScriptableConfig
    {
        protected override void Configure(Entity entity, EntityManager manager, IDefinableContext context)
        {
            context.AddBuffer<PrefabInfo.BakedInnerPathPrefab>(entity);
            context.AddComponentData(entity, new ConfigInfo{ ConfigId = ID });
            context.AddComponentData(entity, new Spawn());
            
            ConfigureStats(entity, context);
            ConfigureChildren(entity, manager, context);
        }

        private void ConfigureChildren(Entity entity, EntityManager manager, IDefinableContext context)
        {
            if (this is not IConfigContainer container) return;
            
            foreach (var iter in container.Childs)
            {
                if (!iter.Enabled) continue;

                var child = context.CreateEntity();
                if (iter.PrefabObject)
                {
                    //context.AppendToBuffer(entity, new PrefabInfo.BakedInnerPathPrefab(child, iter.PrefabObject.GetHierarchyPath()));
                }
                //context.AddComponentData(child, new Root{Value = entity});
                context.AddBuffer<Child>(entity);
                context.AppendToBuffer(entity, new Child{Value = child});
                
                
                context.AddComponentData(entity, new Unity.Transforms.LocalTransform());
                context.AddComponentData(entity, new Unity.Transforms.LocalToWorld());
                
                var buffer = context.AddBuffer<LinkedEntityGroup>(entity);
                context.AppendToBuffer(entity, new LinkedEntityGroup{Value = entity});
                context.AppendToBuffer(entity, new LinkedEntityGroup{Value = child});
                
                context.AddComponentData(child, new Unity.Transforms.LocalTransform());
                context.AddComponentData(child, new Unity.Transforms.PreviousParent(){Value = entity});
                context.AddComponentData(child, new Unity.Transforms.Parent(){Value = entity});
                context.AddComponentData(child, new Unity.Transforms.LocalToWorld());
                context.AddComponentData(child, new Prefab());
                
                ((IConfig)iter.Child).Configure(child, manager, context);
            }
        }

        private void ConfigureStats(Entity entity, IDefinableContext context)
        {
            if (this is not IConfigStats stats) return;
            
            var prepare = context.AddBuffer<PrepareStat>(entity);
            prepare.Add(new PrepareStat { ConfigID = ID });

            context.AddBuffer<Modifier>(entity);
            //context.AddBuffer<Damage.LastDamage>(entity);
            var buff = context.AddBuffer<Stat>(entity);

            stats.Configure(buff);
        }
    }
}
