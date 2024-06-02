using System;

namespace Common.Core
{
    public interface IProgress
    {
        public delegate void OnProgressChange(float value);
        float Value { get; }
        event OnProgressChange OnChange;
    }

    public interface IProgressWritable: IProgress
    {
        float SetProgress(float value);
        float SetDone();
    }

}
