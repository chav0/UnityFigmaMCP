using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    public class ObjectLayoutContext
    {
        public GameObject GameObject { get; }
        public FigmaObject FigmaObject { get; }
        public Transform ParentTransform { get; }
        public FigmaObject RootFrame { get; }
        public FigmaSpriteMap SpriteMap { get; }
        public FigmaObject ParentFigmaObject { get; }

        public ObjectLayoutContext(GameObject gameObject, FigmaObject figmaObject,
            Transform parentTransform, FigmaObject rootFrame, FigmaSpriteMap spriteMap = null,
            FigmaObject parentFigmaObject = null)
        {
            GameObject = gameObject;
            FigmaObject = figmaObject;
            ParentTransform = parentTransform;
            RootFrame = rootFrame;
            SpriteMap = spriteMap;
            ParentFigmaObject = parentFigmaObject;
        }
    }
}
