using System;
using System.Collections.Generic;

namespace Common.UI
{
    public interface IWidgetContainer: IWidget
    {
        IEnumerable<Type> GetWidgetTypes();
    }
}
