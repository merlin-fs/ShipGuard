using System;
using Unity.Entities.Serialization;

namespace Game.Storages
{
    public class LocalStorageUserEndpoint : IStorageEndpoint
    {
        private readonly string m_Path; 
        
        public LocalStorageUserEndpoint(string folderPath)
        {
            m_Path = folderPath;
        }
        
        public BinaryWriter GetWriter()
        {
            var path = $"{m_Path}/User.save";
            return new StreamBinaryWriter(path);
        }

        public BinaryReader GetReader()
        {
            var path = $"{m_Path}/User.save";
            return new StreamBinaryReader(path);
        }
    }
}
