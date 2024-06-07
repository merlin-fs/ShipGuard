using System.Collections.Generic;

using UnityEngine;

namespace Game.Views
{
    public class ViewUnit : MonoBehaviour, IView
    {
        public Transform Transform => transform;

        IEnumerable<T> IView.GetComponents<T>()
        {
            return GetComponents<T>();
        }
    }
}
