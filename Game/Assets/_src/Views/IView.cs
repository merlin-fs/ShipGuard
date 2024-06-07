using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Views
{
    public interface IView
    {
        Transform Transform { get; }
        IEnumerable<T> GetComponents<T>()
            where T : IViewComponent;
    }
}
