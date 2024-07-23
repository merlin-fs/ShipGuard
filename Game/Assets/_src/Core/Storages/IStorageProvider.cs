using Unity.Entities.Serialization;

namespace Game
{
    public interface IStorageEndpoint
    {
        BinaryWriter GetWriter();
        BinaryReader GetReader();
    }
}
