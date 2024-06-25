namespace Common.Defs
{
    public interface IDefinable{}

    public interface IDefinable<T> : IDefinable
        where T : IDef
    {
        void SetDef(RefLink<T> link);
    }
}
