using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityFigmaMCP.Editor
{
    [CreateAssetMenu(fileName = "FigmaComponentMap", menuName = "UnityFigmaMCP/ComponentMap")]
    public class FigmaComponentMap : ScriptableObject
    {
        private const string DefaultAssetPath = "Assets/UnityFigmaMCP/Editor/FigmaComponentMap.asset";
        private const string SearchFilter = "t:FigmaComponentMap";

        [SerializeField] private List<FigmaComponent> components = new();

        public GameObject FindPrefab(string key, string prefabName = null) => FindMatch(key, prefabName)?.Prefab;

        public FigmaComponentMatch FindMatch(string key, string prefabName = null)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var byKey = FindMatch(c => c.id == key, v => v.id == key);
                if (byKey != null)
                    return byKey;
            }

            if (!string.IsNullOrEmpty(prefabName))
                return FindMatch(c => c.name == prefabName, v => v.name == prefabName);

            return null;
        }

        private FigmaComponentMatch FindMatch(Func<FigmaComponent, bool> isComponent,
            Func<FigmaComponentVariant, bool> isVariant)
        {
            foreach (var component in components)
            {
                var variant = component.variants.FirstOrDefault(isVariant);
                if (variant != null && (variant.prefab != null || component.prefab != null))
                    return new FigmaComponentMatch(component, variant);
            }

            var match = components.FirstOrDefault(c => c.prefab != null && isComponent(c));
            return match != null ? new FigmaComponentMatch(match, null) : null;
        }

        public FigmaComponent FindComponent(string key, string prefabName = null)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var byKey = components.FirstOrDefault(c => c.id == key);
                if (byKey != null)
                    return byKey;
            }

            if (!string.IsNullOrEmpty(prefabName))
            {
                var byName = components.FirstOrDefault(c => c.name == prefabName);
                if (byName != null)
                    return byName;
            }

            return null;
        }

        public void AddComponent(string key, string prefabName, GameObject prefab)
        {
            var existing = FindComponent(key, prefabName);
            if (existing != null)
            {
                existing.id = key;
                existing.name = prefabName;
                if (prefab != null)
                    existing.prefab = prefab;
            }
            else
            {
                components.Add(new FigmaComponent(key, prefabName, prefab));
            }

            ForceSave();
        }

        public void AddVariant(string componentKey, string componentName,
            string variantKey, string variantName, GameObject variantPrefab)
        {
            var component = FindComponent(componentKey, componentName);
            if (component == null)
            {
                component = new FigmaComponent(componentKey, componentName, null);
                components.Add(component);
            }

            component.AddVariant(variantKey, variantName, variantPrefab);
            
            ForceSave();
        }

        public void Clean()
        {
            for (var i = components.Count - 1; i >= 0; i--)
            {
                if (components[i].prefab == null &&
                    (components[i].variants.Count == 0 || components[i].variants.All(x => x.prefab == null)))
                    components.RemoveAt(i);
            }

            ForceSave();
        }

        private void ForceSave()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        public static FigmaComponentMap GetOrCreate()
        {
            var guids = AssetDatabase.FindAssets(SearchFilter);

            if (guids.Length > 0)
                return AssetDatabase.LoadAssetAtPath<FigmaComponentMap>(AssetDatabase.GUIDToAssetPath(guids[0]));

            if (!AssetDatabase.IsValidFolder("Assets/UnityFigmaMCP"))
                AssetDatabase.CreateFolder("Assets", "UnityFigmaMCP");

            if (!AssetDatabase.IsValidFolder("Assets/UnityFigmaMCP/Editor"))
                AssetDatabase.CreateFolder("Assets/UnityFigmaMCP", "Editor");

            var map = CreateInstance<FigmaComponentMap>();
            AssetDatabase.CreateAsset(map, DefaultAssetPath);
            AssetDatabase.SaveAssets();

            return map;
        }
    }

    [Serializable]
    public class FigmaComponent
    {
        public string name;
        public string id;
        public GameObject prefab;
        public List<FigmaComponentVariant> variants = new();

        public FigmaComponent(string id, string name, GameObject prefab)
        {
            this.id = id;
            this.name = name;
            this.prefab = prefab;
        }

        public FigmaComponentVariant FindVariant(string key, string variantName = null)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var byKey = variants.FirstOrDefault(v => v.id == key);
                if (byKey != null)
                    return byKey;
            }

            if (!string.IsNullOrEmpty(variantName))
            {
                var byName = variants.FirstOrDefault(v => v.name == variantName);
                if (byName != null)
                    return byName;
            }

            return null;
        }

        public void AddVariant(string key, string variantName, GameObject variantPrefab)
        {
            var existing = FindVariant(key, variantName);
            if (existing != null)
            {
                existing.id = key;
                existing.name = variantName;
                if (variantPrefab != null)
                    existing.prefab = variantPrefab;
                return;
            }

            variants.Add(new FigmaComponentVariant(key, variantName, variantPrefab));
        }
    }

    [Serializable]
    public class FigmaComponentVariant
    {
        public string name;
        public string id;
        public GameObject prefab;

        public FigmaComponentVariant(string id, string name, GameObject prefab)
        {
            this.id = id;
            this.name = name;
            this.prefab = prefab;
        }
    }
}
