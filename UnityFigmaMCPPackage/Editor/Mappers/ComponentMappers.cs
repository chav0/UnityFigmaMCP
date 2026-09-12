namespace UnityFigmaMCP.Editor
{
    internal sealed class ComponentMappers
    {
        public readonly RectTransformMapper Rect = new();
        public readonly ImageMapper Img = new();
        public readonly TextMapper Text = new();
        public readonly HorizontalLayoutMapper HLayout = new();
        public readonly VerticalLayoutMapper VLayout = new();
        public readonly GridLayoutMapper Grid = new();
        public readonly ContentSizeFitterMapper Fitter = new();

        public readonly IComponentMapper[] All;

        public ComponentMappers()
        {
            All = new IComponentMapper[]
            {
                Rect, Img, Text,
                HLayout, VLayout, Grid,
                Fitter
            };
        }
    }
}
