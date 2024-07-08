using System.Threading.Tasks;

using Common.Core;
using Common.Core.Loading;

using Game.UI;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Game.Core.Loading
{
    public class CommandLoadLocation : ICommandProgress, ICommandNewContainer
    {
        private readonly CommandContainer m_CommandContainer;

        public CommandLoadLocation(IContainer container, string locationName)
        {
            m_CommandContainer = new CommandContainer(container)
                .Add(new CommandUiShow<UI.Loading>(UILayer.Loading).AsItem())
                .Add(new CommandLoadStorage($"{Application.persistentDataPath}/saveData.test").AsItem())
                .Add(new CommandLoadSceneLocation(locationName).AsItem())
                .Add(new CommandUiHide<UI.Loading>().AsItem(1))
                .Add(new CommandSpawnPlayer().AsItem(2));
        }
            
        public Task Execute()
        {
            return m_CommandContainer.Execute();
        }

        public IContainer GetContainer()
        {
            return m_CommandContainer.GetContainer();
        }

        public float GetProgress()
        {
            return m_CommandContainer.GetProgress();
        }
    }
}
