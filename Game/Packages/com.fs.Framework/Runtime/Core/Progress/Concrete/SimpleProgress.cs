using System;
using System.Threading;

namespace Common.Core.Progress
{
    public sealed class SimpleProgress : IProgressWritable
    {
        private volatile float m_Value = 0;
        private event IProgress.OnProgressChange m_OnProgressChange;
        private IProgressWritable Self => this;

        public SimpleProgress() { }

        #region IProgress
        float IProgress.Value
        {
            get => m_Value;
        }

        float IProgressWritable.SetProgress(float value)
        {
            m_Value = Interlocked.Exchange(ref m_Value, value);
            m_OnProgressChange?.Invoke(m_Value);
            return m_Value;
        }

        float IProgressWritable.SetDone()
        {
            return Self.SetProgress(1);
        }

        event IProgress.OnProgressChange IProgress.OnChange
        {
            add => m_OnProgressChange += value;
            remove => m_OnProgressChange -= value;
        }
        #endregion
    }
}
