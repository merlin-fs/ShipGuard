using System;

namespace Common.Core
{
    public interface IIdentifiable<out T>
    {
        T ID { get; }
    }
}