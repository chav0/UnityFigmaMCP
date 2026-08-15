using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public static class PrefabEditOps
    {
        public const string Create = "create";
        public const string Delete = "delete";
        public const string Reparent = "reparent";
        public const string SetActive = "setActive";

        public const string RectTransform = "rectTransform";
        public const string Image = "image";
        public const string Text = "text";
        public const string HorizontalLayout = "horizontalLayout";
        public const string VerticalLayout = "verticalLayout";
        public const string GridLayout = "gridLayout";
        public const string ContentSizeFitter = "contentSizeFitter";
    }

    public class PrefabEdit
    {
        [Description("Operation to perform. Component ops read the matching payload field and add the component if missing — set Remove=true to remove it instead: \"rectTransform\", \"image\", \"text\", \"horizontalLayout\", \"verticalLayout\", \"gridLayout\", \"contentSizeFitter\". Graph ops: \"create\" (Path is the parent, Name is required), \"delete\", \"reparent\" (NewParentPath is required), \"setActive\" (Active is required).")]
        public string Op { get; set; }

        [Description("Path to the target object inside the prefab, relative to the root (e.g. \"Header/Title\"). Null or empty targets the prefab root. For \"create\" this is the parent to add the new object under.")]
        public string Path { get; set; }

        [Description("For component ops: remove the component instead of applying the payload. Default false.")]
        public bool Remove { get; set; }

        [Description("For \"create\": name of the new GameObject.")]
        public string Name { get; set; }

        [Description("For \"reparent\": path of the destination parent. Empty string moves the object to the root.")]
        public string NewParentPath { get; set; }

        [Description("For \"reparent\": position among the new parent's children. Omit to append last.")]
        public int? SiblingIndex { get; set; }

        [Description("For \"setActive\": whether the GameObject should be active.")]
        public bool? Active { get; set; }

        [Description("Payload for op \"rectTransform\".")]
        public RectTransformComponent RectTransform { get; set; }

        [Description("Payload for op \"image\".")]
        public ImageComponent Image { get; set; }

        [Description("Payload for op \"text\".")]
        public TextComponent Text { get; set; }

        [Description("Payload for op \"horizontalLayout\".")]
        public HorizontalLayoutComponent HorizontalLayout { get; set; }

        [Description("Payload for op \"verticalLayout\".")]
        public VerticalLayoutComponent VerticalLayout { get; set; }

        [Description("Payload for op \"gridLayout\".")]
        public GridLayoutComponent GridLayout { get; set; }

        [Description("Payload for op \"contentSizeFitter\".")]
        public ContentSizeFitterComponent ContentSizeFitter { get; set; }
    }
}
