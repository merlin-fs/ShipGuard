using System;

using Game.Core.Defs;

using Newtonsoft.Json.Linq;

using Unity.Entities;

using UnityEngine;

namespace Game.Core.Spawns
{
    public partial class Spawner
    {
        public partial class Builder
        {
            public Builder WithData(JToken token)
            {
                foreach (var iter in token)
                {
                    var typeName = ((JProperty)iter).Name;
                    var type = Type.GetType(typeName);
                    if (type == null)
                        throw new ArgumentNullException($"Type {typeName} not found");

                    if (TypeManager.IsSystemType(type) || type == typeof(PrefabInfo))
                        continue;

                    var component = iter.ToObject(type);
                    if (component != null)
                        WithComponent(component, type);
                }

                return this;
            }
        }
    }
}
