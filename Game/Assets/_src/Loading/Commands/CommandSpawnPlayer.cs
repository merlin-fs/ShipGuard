using System;
using System.Linq;
using System.Threading.Tasks;

using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Game.Core.Repositories;
using Game.Model.Units;
using Game.Views;

using Reflex.Attributes;

namespace Game.Core.Loading
{
    public class CommandSpawnPlayer : ICommand
    {
        [Inject] private UnitManager m_UnitManager;
        [Inject] private LocationViewRepository m_LocationViewRepository;

        public Task Execute()
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
        
                var locationPoint = m_LocationViewRepository.Find().First();
                var locationPointId = locationPoint.GetComponents<ViewComponentIdentifiable>().First().ID;
                
                m_UnitManager.SpawnPlayer(locationPointId);
            }).AsTask();
        }
    }
}