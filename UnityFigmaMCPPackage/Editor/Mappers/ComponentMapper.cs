using UnityEngine;
using UnityFigmaMCP.Common;
using Object = UnityEngine.Object;

namespace UnityFigmaMCP.Editor
{
    internal abstract class ComponentMapper<TUnity, TDto> : IComponentMapper where TUnity : Component
    {
        public abstract TDto Read(TUnity component);
        public abstract void Write(TUnity component, TDto dto);
        protected abstract void Assign(UnityObject target, TDto dto);

        public void ReadInto(GameObject gameObject, UnityObject target)
        {
            var component = gameObject.GetComponent<TUnity>();
            if (component != null)
                Assign(target, Read(component));
        }

        public virtual void Apply(GameObject gameObject, TDto dto)
        {
            var component = gameObject.GetComponent<TUnity>();
            if (component == null)
                component = gameObject.AddComponent<TUnity>();

            Write(component, dto);
        }

        public virtual void Remove(GameObject gameObject)
        {
            var component = gameObject.GetComponent<TUnity>();
            if (component != null)
                Object.DestroyImmediate(component);
        }
    }
}
