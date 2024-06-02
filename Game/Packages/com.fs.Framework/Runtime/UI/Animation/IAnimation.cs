using System;
using System.Threading.Tasks;

namespace Common.UI
{
    public interface IAnimation
    {
        void Kill();
        Task Play();
    }
}