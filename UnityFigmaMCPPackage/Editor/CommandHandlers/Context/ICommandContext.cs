namespace UnityFigmaMCP.Editor
{
    internal interface ICommandContext
    {
        FigmaAutoLayoutSettings LayoutSettings { get; }
        ComponentMappers Mappers { get; }
    }
}
