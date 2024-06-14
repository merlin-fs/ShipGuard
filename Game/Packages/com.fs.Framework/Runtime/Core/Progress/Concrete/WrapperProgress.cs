using System;

namespace Common.Core.Progress
{
    public sealed class WrapperProgress : IProgress
    {
        private readonly Func<float> m_Getter;

        public WrapperProgress(Func<float> getter)
        {
            m_Getter = getter;
        }

        #region IProgress
        float IProgress.Value => m_Getter.Invoke();
        #endregion
    }
}
