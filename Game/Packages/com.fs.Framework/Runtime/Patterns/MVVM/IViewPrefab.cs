using System.Threading.Tasks;
using UnityEngine;

namespace Common.Core
{
    public interface IViewPrefab
    {
        GameObject GetViewPrefab();
        Task PreloadPrefab();
    }
}
