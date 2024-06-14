using System;

namespace Common.Core
{
    public interface IProgress
    {
        float Value { get; }
    }

    public interface IProgressWritable: IProgress
    {
        float SetProgress(float value);
        float SetDone();
    }

}
