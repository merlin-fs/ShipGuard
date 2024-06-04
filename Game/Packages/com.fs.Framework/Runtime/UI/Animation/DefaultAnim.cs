using Common.Core;
using UnityEngine;

namespace Common.UI.Windows
{
    public class DefaultAnim : MonoBehaviour, IAdditionalBehaviour, IInitialization
    {
        private IWindowManager m_WindowManager;
        
        public void Play(IWindow.AnimationMode mode, float time)
        {
            switch (mode)
            {
                case IWindow.AnimationMode.Hide:
                case IWindow.AnimationMode.Close:
                    m_WindowManager.SetDarkVisible(false);
                    break;
                case IWindow.AnimationMode.Open:
                case IWindow.AnimationMode.Show:
                    m_WindowManager.SetDarkVisible(true);
                    break;
            }
        }

        public void Initialization(IContainer container)
        {
            m_WindowManager = container.Resolve<IWindowManager>();
        }
    }
}
