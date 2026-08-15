namespace UnityFigmaMCP.Common
{
    public class UnityObject
    {
        public string Name;
        public string Path;
        public bool Active;
        public GameObjectComponent GameObject;
        public RectTransformComponent RectTransform;
        public ImageComponent Image;
        public ButtonComponent Button;
        public TextComponent Text;
        public HorizontalLayoutComponent HorizontalLayout;
        public VerticalLayoutComponent VerticalLayout;
        public GridLayoutComponent GridLayout;
        public ContentSizeFitterComponent ContentSizeFitter;
        public MaskComponent Mask;
        public UnityObject[] Children;
    }
}
