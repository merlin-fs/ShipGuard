using System.Threading.Tasks;

using Common.Core;
using Common.Core.Loading;

using Game.UI;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Game.Core.Loading
{
    public class CommandCloseLocation : ICommandProgress, ICommandNewContainer
    {
        private readonly CommandContainer m_CommandContainer;

        public CommandCloseLocation(IContainer container)
        {
            m_CommandContainer = new CommandContainer(container)
                .Add(new CommandUiShow<UI.Loading>(UILayer.Loading).AsItem())
                .Add(new CommandSaveStorage($"{Application.persistentDataPath}/saveData.test").AsItem())
                .Add(new CommandCloseSceneLocation().AsItem())
                .Add(new CommandUiHide<UI.Loading>().AsItem(1));
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
