using System;
using System.Threading;

namespace Common.Core.Progress
{
    public sealed class SimpleProgress : IProgressWritable
    {
        private volatile float m_Value = 0;
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
            return m_Value;
        }

        float IProgressWritable.SetDone()
        {
            return Self.SetProgress(1);
        }
        #endregion
    }
}
