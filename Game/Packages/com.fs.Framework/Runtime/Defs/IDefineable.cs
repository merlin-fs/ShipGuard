using System;

namespace Common.Defs
{
    public interface IDefinable { }

    public record GetterDef<T>(Func<T> Getter)
        where T : IDef
    {
        public Func<T> Getter { get; } = Getter;
    }
}