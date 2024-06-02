using System.Threading.Tasks;
using UnityEngine;

namespace Common.Core
{
    public interface IViewPrefab
    {
        public Task<GameObject> GetViewPrefab();
    }
}
