using System;
using UnityEngine;

namespace Common.Core
{
    public interface IView : IDisposable
    {
        IViewModel DataSource { get; }
        void Initialize(IViewModel DataSource);

        GameObject GameObject { get; }
    }

    public interface IView<T> : IView
        where T : IViewModel
    {
        new T DataSource { get; }
    }
}