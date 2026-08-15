using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal interface IComponentMapper
    {
        void ReadInto(GameObject gameObject, UnityObject target);
    }
}
