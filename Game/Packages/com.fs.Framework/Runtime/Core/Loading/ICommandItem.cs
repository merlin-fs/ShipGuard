namespace Common.Core.Loading
{

    public interface ICommandItem
    {
        ICommand Command { get; }
        int[] Dependencies { get; }
    }

    public static class CommandItemExt
    {
        public static ICommandItem AsItem(this ICommand command, params int[] dependencies)
        {
            var item = default(CommandItem);
            item.Command = command;
            item.Dependencies = dependencies;
            return item;
        }

        private struct CommandItem : ICommandItem
        {
            public ICommand Command { get; set; }
            public int[] Dependencies { get; set;}
        }
    }
}