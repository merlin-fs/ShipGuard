using System.Threading.Tasks;
using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Game.Core.Defs;
using Game.Core.Repositories;

using Reflex.Attributes;

using Unity.Entities;
using Unity.Entities.Serialization;

namespace Game.Core.Loading
{
    public class CommandSaveStorage : ICommandProgress
    {
        [Inject] private SerializeManager m_SerializeManager;
        
        private string m_Path;

        public float GetProgress()
        {
            return 1;
        }

        public CommandSaveStorage(){}

        public CommandSaveStorage(string path) => m_Path = path;
        
        public Task Execute()
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
                
                using (var writer = new StreamBinaryWriter(m_Path))
                {
                    m_SerializeManager.SerializeWorld(World.DefaultGameObjectInjectionWorld, writer);
                }
            }).AsTask();
        }
    }
}
