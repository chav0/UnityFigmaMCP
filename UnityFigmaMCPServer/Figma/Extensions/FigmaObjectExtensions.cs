using System.Linq;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Server.Figma
{
    internal static class FigmaObjectExtensions
    {
        internal static FigmaObject FindNode(this FigmaObject node, string nodeId)
        {
            if (node == null) 
                return null;
        
            if (node.id == nodeId) 
                return node;
        
            if (node.children == null) 
                return null;
            
            foreach (var child in node.children)
            {
                var found = child.FindNode(nodeId);
                if (found != null) 
                    return found;
            }

            return null;
        }
    
        internal static object ToSlim(this FigmaObject node)
        {
            if (node == null) return null;

            return new
            {
                id = node.id,
                name = node.name,
                type = node.type.ToString(),
                children = node.children?.Select(ToSlim).ToArray()
            };
        }
    }}
