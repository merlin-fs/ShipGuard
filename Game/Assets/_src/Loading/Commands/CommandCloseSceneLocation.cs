using System;
using System.Threading.Tasks;

using Common.Core;
using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Reflex.Attributes;

using UnityEngine;

namespace Game.Core.Loading
{
    public class CommandCloseSceneLocation : ICommandProgress, ICommandNewContainer, IProgress<float>
    {
        [Inject] private LocationManager m_LocationManager;

        private float m_Progress;
        private IContainer m_Container;

        public CommandCloseSceneLocation(){}

        public float GetProgress()
        {
            return m_Progress;
        }

        public IContainer GetContainer()
        {
            return m_Container;
        }

        public Task Execute()
        {
            return UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.SwitchToMainThread();
                await m_LocationManager.CloseCurrentLocation(null);
            }).AsTask();
        }

        public void Report(float value)
        {
            m_Progress = value;
        }
    }
}