namespace Common.Defs
{
    public interface IDef{}
    
    public interface IDef<T>: IDef
        where T : IDefinable
    {
    }
}