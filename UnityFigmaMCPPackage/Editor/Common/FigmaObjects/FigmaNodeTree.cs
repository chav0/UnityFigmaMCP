using System.Collections.Generic;

namespace UnityFigmaMCP.Common
{
    public class FigmaNodeTree
    {
        public string name;
        public Dictionary<string, FigmaNodeData> nodes;
    }

    public class FigmaNodeData
    {
        public FigmaObject document;
        public Dictionary<string, FigmaComponentMeta> components;
    }
}
