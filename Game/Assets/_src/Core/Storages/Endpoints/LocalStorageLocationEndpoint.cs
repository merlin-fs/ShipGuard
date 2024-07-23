using Game.Model.Locations;

using Unity.Entities.Serialization;

namespace Game.Storages
{
    public class LocalStorageLocationEndpoint : IStorageEndpoint
    {
        private readonly string m_Path; 
        private readonly LocationSceneItem m_LocationSceneItem;
        
        public LocalStorageLocationEndpoint(string folderPath, LocationSceneItem locationSceneItem)
        {
            m_Path = folderPath;
            m_LocationSceneItem = locationSceneItem;
        }
        
        public BinaryWriter GetWriter()
        {
            var path = $"{m_Path}/{m_LocationSceneItem.Name}.save";
            return new StreamBinaryWriter(path);
        }

        public BinaryReader GetReader()
        {
            var path = $"{m_Path}/{m_LocationSceneItem.Name}.save";
            return new StreamBinaryReader(path);
        }
    }
}
