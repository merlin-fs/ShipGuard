using System;

using Unity.Entities;

namespace Game.Core.Placements
{
    public partial struct Placement
    {
        public partial struct Layers
        {
            public struct Floor : ILayer
            {
                private Entity m_Entity;
                public Entity Entity { get => m_Entity; set => m_Entity = value; }
            }
        }
    }
}