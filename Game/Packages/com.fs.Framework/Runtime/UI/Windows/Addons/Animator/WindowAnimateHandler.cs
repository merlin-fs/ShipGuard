using System;
using UnityEngine;
using System.Threading.Tasks;

namespace Common.UI.Windows
{
    public class WindowAnimateHandler : IAnimation
    {
        public void Kill() 
        { }

        public Task Play()
        {
            return Task.CompletedTask;
        }
    }
}