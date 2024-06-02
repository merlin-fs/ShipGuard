using System;

namespace Common.UI.Windows
{
    public interface IAdditionalBehaviour 
    {
        void Play(IWindow.AnimationMode mode, float time);
    }
}
