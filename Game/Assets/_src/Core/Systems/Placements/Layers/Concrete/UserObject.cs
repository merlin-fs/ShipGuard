using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Core.Placements
{
    public partial struct Placement
    {
        public partial struct Layers
        {
            public struct UserObject : ILayer
            {
                private Entity m_Entity;
                public Entity Entity { get => m_Entity; set => m_Entity = value; }
                /*
                public class Validator : ILayerValidator
                {
                    public bool CanPlace(Aspect aspect, int2 pos, Entity entity)
                    {
                        var canPlace = aspect.TryGetObject<Layers.Floor>(pos, out var _) &&
                                       !aspect.TryGetObject<Layers.Structure>(pos, out var _) &&
                                       !aspect.TryGetObject<Layers.Door>(pos, out var _);
                        return canPlace;
                    }
                }
                */
            }
        }
    }
}