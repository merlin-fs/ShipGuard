using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Common.UI
{
    public interface IWidget
    {
        void Bind(UIDocument document);
    }
}
