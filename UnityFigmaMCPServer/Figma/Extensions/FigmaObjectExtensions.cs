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
    
        internal static object ToSlim(this FigmaObject node, int? depth = null)
        {
            if (node == null) return null;

            var expand = depth == null || depth > 0;
            var hasChildren = node.children != null && node.children.Length > 0;

            return new
            {
                id = node.id,
                name = node.name,
                type = node.type.ToString(),
                children = expand && hasChildren ? node.children.Select(child => child.ToSlim(depth - 1)).ToArray() : null,
                childCount = !expand && hasChildren ? node.children.Length : (int?)null
            };
        }
    }}
