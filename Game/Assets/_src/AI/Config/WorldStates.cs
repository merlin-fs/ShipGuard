using System.Linq;
using System.Reflection.Ext;
using System.Collections.Generic;

using Game.Core;

using Unity.Entities;

namespace Game.AI.GOAP
{
    public partial struct Logic
    {
        public partial class LogicDef
        {
            private Dictionary<EnumHandle, WorldState> m_StateMapping = new (10);
            public int WorldStateMapping(EnumHandle enumHandle) => m_StateMapping[enumHandle].Index;
            
            public void SetupWorldStates(ref DynamicBuffer<WorldStateComponent> buffer)
            {
                buffer.ResizeUninitialized(m_StateMapping.Count);
                foreach (var iter in m_StateMapping)
                {
                    buffer[iter.Value.Index] = new WorldStateComponent {Value = iter.Value.Initialize};
                }
            }

            private void WorldStatesInitialization()
            {
                var types = typeof(IStateData).GetDerivedTypes(true)
                    .SelectMany(t => t.GetNestedTypes())
                    .Where(t => t.IsEnum && t.Name == "State");

                foreach (var iter in types)
                {
                    foreach (var e in iter.GetEnumValues())
                    {
                        var value = EnumHandle.FromObjectEnum(e);
                        m_StateMapping.Add(value, new WorldState 
                        {
                            Index = m_StateMapping.Count,
                            Initialize = WorldStateHandle.FromHandle(value, false),
                        });
                    }
                }
            }

            
            private struct WorldState
            {
                public int Index;
                public WorldStateHandle Initialize;
            }
        }
    }
}
