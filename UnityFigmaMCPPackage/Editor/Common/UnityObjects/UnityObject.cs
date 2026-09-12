namespace UnityFigmaMCP.Common
{
    public class UnityObject
    {
        public string Name;
        public string Path;
        public bool? Active;
        public GameObjectComponent GO;
        public RectTransformComponent Rect;
        public ImageComponent Img;
        public ButtonComponent Btn;
        public TextComponent Text;
        public HorizontalLayoutComponent HLayout;
        public VerticalLayoutComponent VLayout;
        public GridLayoutComponent Grid;
        public ContentSizeFitterComponent Fitter;
        public MaskComponent Mask;
        public UnityObject[] Children;
    }
}
