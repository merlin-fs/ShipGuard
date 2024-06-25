using UnityEngine;

namespace Common.Core
{
    public interface ICastObject
    {
        T Cast<T>();
    }
}
