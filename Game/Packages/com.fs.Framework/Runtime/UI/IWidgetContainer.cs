using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Common.UI
{
    public interface IWidgetContainer: IWidget
    {
        IEnumerable<Type> GetWidgetTypes();
    }
}
