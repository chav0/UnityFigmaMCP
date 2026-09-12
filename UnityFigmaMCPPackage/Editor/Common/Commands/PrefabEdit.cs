using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public static class PrefabEditOps
    {
        public const string Create = "create";
        public const string Delete = "delete";
        public const string Reparent = "reparent";
        public const string SetActive = "setActive";
        public const string Instantiate = "instantiate";

        public const string RectTransform = "rect";
        public const string Image = "img";
        public const string Text = "text";
        public const string HorizontalLayout = "hLayout";
        public const string VerticalLayout = "vLayout";
        public const string GridLayout = "grid";
        public const string ContentSizeFitter = "fitter";
    }

    public class PrefabEdit
    {
        [Description("Operation: \"rect\", \"img\", \"text\", \"hLayout\", \"vLayout\", \"grid\", \"fitter\" (component ops — set Remove=true to remove), \"create\" (Path=parent, Name required), \"instantiate\" (Path=parent, Prefab=asset path), \"delete\", \"reparent\" (NewParentPath required), \"setActive\" (Active required).")]
        public string Op { get; set; }

        [Description("Path to the target object inside the prefab (e.g. \"Header/Title\"). Null or empty targets the root.")]
        public string Path { get; set; }

        [Description("Remove the component instead of applying the payload")]
        public bool Remove { get; set; }

        [Description("For \"create\": name of the new GameObject")]
        public string Name { get; set; }

        [Description("For \"instantiate\": prefab asset path (e.g. \"Assets/UI/Prefabs/Card.prefab\")")]
        public string Prefab { get; set; }

        [Description("For \"reparent\": destination parent path. Empty string = root.")]
        public string NewParentPath { get; set; }

        [Description("For \"reparent\": sibling index. Omit to append last.")]
        public int? SiblingIndex { get; set; }

        [Description("For \"setActive\": whether the GameObject should be active")]
        public bool? Active { get; set; }

        [Description("Payload for op \"rect\"")]
        public RectTransformComponent Rect { get; set; }

        [Description("Payload for op \"img\"")]
        public ImageComponent Img { get; set; }

        [Description("Payload for op \"text\"")]
        public TextComponent Text { get; set; }

        [Description("Payload for op \"hLayout\"")]
        public HorizontalLayoutComponent HLayout { get; set; }

        [Description("Payload for op \"vLayout\"")]
        public VerticalLayoutComponent VLayout { get; set; }

        [Description("Payload for op \"grid\"")]
        public GridLayoutComponent Grid { get; set; }

        [Description("Payload for op \"fitter\"")]
        public ContentSizeFitterComponent Fitter { get; set; }
    }
}
