using System.Threading.Tasks;

using Common.Core.Loading;

using Game.Storages;

using Reflex.Attributes;

namespace Game.Core.Loading
{
    public class CommandSaveStorage : ICommandProgress
    {
        [Inject] private StorageManager m_StorageManager;

        public float GetProgress()
        {
            return 1;
        }

        public Task Execute()
        {
            return m_StorageManager.Save();
        }
    }
}
