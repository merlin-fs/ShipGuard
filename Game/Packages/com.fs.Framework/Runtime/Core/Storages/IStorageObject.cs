using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Core
{
    public interface IStorageObject
    {
        string Type { get; }
        IData Data { get; }
    }
}
