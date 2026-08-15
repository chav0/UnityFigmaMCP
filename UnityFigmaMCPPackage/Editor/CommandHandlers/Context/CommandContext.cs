namespace UnityFigmaMCP.Editor
{
    internal sealed class CommandContext : ICommandContext
    {
        public FigmaAutoLayoutSettings LayoutSettings { get; }
        public ComponentMappers Mappers { get; }

        public CommandContext(FigmaAutoLayoutSettings layoutSettings, ComponentMappers mappers)
        {
            LayoutSettings = layoutSettings;
            Mappers = mappers;
        }
    }
}
