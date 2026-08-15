using System;
using UnityEditor;
using UnityEngine;

namespace UnityFigmaMCP.Editor
{
    internal sealed class PrefabEditScope : IDisposable
    {
        public GameObject Target { get; }
        public GameObject Root { get; }
        public string PrefabPath { get; }

        private PrefabEditScope(GameObject target, GameObject root, string prefabPath)
        {
            Target = target;
            Root = root;
            PrefabPath = prefabPath;
        }

        public static PrefabEditScope Create(string prefabPath, string objectPath = null)
        {
            if (string.IsNullOrEmpty(prefabPath))
                throw new Exception("Prefab path is required.");

            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            if (root == null)
                throw new Exception($"Prefab not found at '{prefabPath}'");

            var target = root;

            if (!string.IsNullOrEmpty(objectPath))
            {
                var child = root.transform.Find(objectPath);
                if (child == null)
                {
                    PrefabUtility.UnloadPrefabContents(root);
                    throw new Exception($"Child '{objectPath}' not found in '{prefabPath}'");
                }

                target = child.gameObject;
            }

            return new PrefabEditScope(target, root, prefabPath);
        }

        public void Save()
        {
            PrefabUtility.SaveAsPrefabAsset(Root, PrefabPath);
        }

        public GameObject FindInContext(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return Root;
            return Root.transform.Find(relativePath)?.gameObject;
        }

        public void Dispose()
        {
            PrefabUtility.UnloadPrefabContents(Root);
        }
    }
}
