namespace UnityFigmaMCP.Editor
{
    internal sealed class ComponentMappers
    {
        public readonly RectTransformMapper RectTransform = new();
        public readonly ImageMapper Image = new();
        public readonly TextMapper Text = new();
        public readonly HorizontalLayoutMapper HorizontalLayout = new();
        public readonly VerticalLayoutMapper VerticalLayout = new();
        public readonly GridLayoutMapper GridLayout = new();
        public readonly ContentSizeFitterMapper ContentSizeFitter = new();

        public readonly IComponentMapper[] All;

        public ComponentMappers()
        {
            All = new IComponentMapper[]
            {
                RectTransform, Image, Text,
                HorizontalLayout, VerticalLayout, GridLayout,
                ContentSizeFitter
            };
        }
    }
}
