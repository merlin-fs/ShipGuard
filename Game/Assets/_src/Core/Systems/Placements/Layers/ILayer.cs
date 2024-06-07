using Unity.Entities;

namespace Game.Core.Placements
{
    public partial struct Placement
    {
        public partial struct Layers
        {
            public interface ILayer : IBufferElementData
            {
                Entity Entity { get; set; }
            }
        }
    }
}
