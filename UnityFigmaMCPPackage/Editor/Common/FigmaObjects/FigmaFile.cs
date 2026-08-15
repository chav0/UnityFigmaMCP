using System.Collections.Generic;

namespace UnityFigmaMCP.Common
{
    public class FigmaFile
    {
        public string name;
        public Dictionary<string, FigmaComponentMeta> components;
        public FigmaObject root;

        public string GetComponentKey(string nodeId)
        {
            if (components != null && components.TryGetValue(nodeId, out var meta))
                return meta.key;
            return null;
        }

        public string GetComponentSetId(string nodeId)
        {
            if (components != null && components.TryGetValue(nodeId, out var meta))
                return meta.componentSetId;
            return null;
        }
    }

    public class FigmaComponentMeta
    {
        public string key;
        public string name;
        public string description;
        public string componentSetId;
    }
}
