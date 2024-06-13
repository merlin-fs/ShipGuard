using System;

namespace Reflex.Exceptions
{
    internal sealed class FieldInjectorException : Exception
    {
        public FieldInjectorException(string message) : base(message)
        {
        }
    }
}