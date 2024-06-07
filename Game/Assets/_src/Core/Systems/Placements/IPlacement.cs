using Unity.Entities;
using Unity.Mathematics;

namespace Game.Core.Placements
{
    public interface IPlacement
    {
        int2 Size { get; }
        float3 Pivot { get; }
        TypeIndex Layer  { get; }
    }
}
