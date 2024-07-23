using System.Threading.Tasks;
using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Game.Storages;

using Reflex.Attributes;

using UnityEngine;

namespace Game.Core.Loading
{
    public class CommandLoadStorage : ICommandProgress
    {
        [SerializeField] private string locationName;
        [Inject] private StorageManager m_StorageManager;
        [Inject] private LocationManager m_LocationManager;
        
        private string m_Path;

        public CommandLoadStorage(string locationName)
        {
            this.locationName = locationName;
        }
        
        public float GetProgress()
        {
            return 1;
        }

        public Task Execute()
        {
            return UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.SwitchToMainThread();
                m_StorageManager.SetContainerEndpoint<StorageLocation>(new LocalStorageLocationEndpoint(Application.persistentDataPath, m_LocationManager.GetLocationItem(locationName)));
                await m_StorageManager.Load();
            }).AsTask();
        }
    }
}
