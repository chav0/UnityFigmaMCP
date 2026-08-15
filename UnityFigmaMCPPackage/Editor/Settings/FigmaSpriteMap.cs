using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityFigmaMCP.Editor
{
    [CreateAssetMenu(fileName = "FigmaSpriteMap", menuName = "UnityFigmaMCP/SpriteMap")]
    public class FigmaSpriteMap : ScriptableObject
    {
        private const string DefaultAssetPath = "Assets/UnityFigmaMCP/Editor/FigmaSpriteMap.asset";
        private const string SearchFilter = "t:FigmaSpriteMap";

        [SerializeField] private List<FigmaIconEntry> entries = new();

        public Sprite Find(string nameOrId)
        {
            if (string.IsNullOrEmpty(nameOrId))
                return null;

            return entries.FirstOrDefault(e =>
                e.sprite != null && (e.name == nameOrId || e.id == nameOrId || e.figmaName == nameOrId))?.sprite;
        }

        public string FindId(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
                return null;

            return entries.FirstOrDefault(e => e.name == spriteName)?.id;
        }

        public void Add(string spriteName, Sprite sprite, string id = null, string figmaName = null)
        {
            var existing = entries.FirstOrDefault(e => e.name == spriteName);
            if (existing != null)
            {
                existing.sprite = sprite;
                if (!string.IsNullOrEmpty(id))
                    existing.id = id;
                if (!string.IsNullOrEmpty(figmaName))
                    existing.figmaName = figmaName;
            }
            else
            {
                entries.Add(new FigmaIconEntry(spriteName, sprite, id, figmaName));
            }

            ForceSave();
        }

        private void ForceSave()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        public static FigmaSpriteMap GetOrCreate()
        {
            var guids = AssetDatabase.FindAssets(SearchFilter);

            if (guids.Length > 0)
                return AssetDatabase.LoadAssetAtPath<FigmaSpriteMap>(AssetDatabase.GUIDToAssetPath(guids[0]));

            if (!AssetDatabase.IsValidFolder("Assets/UnityFigmaMCP"))
                AssetDatabase.CreateFolder("Assets", "UnityFigmaMCP");

            if (!AssetDatabase.IsValidFolder("Assets/UnityFigmaMCP/Editor"))
                AssetDatabase.CreateFolder("Assets/UnityFigmaMCP", "Editor");

            var map = CreateInstance<FigmaSpriteMap>();
            AssetDatabase.CreateAsset(map, DefaultAssetPath);
            AssetDatabase.SaveAssets();

            return map;
        }
    }

    [Serializable]
    public class FigmaIconEntry
    {
        public string name;
        public string id;
        public string figmaName;
        public Sprite sprite;

        public FigmaIconEntry(string name, Sprite sprite, string id = null, string figmaName = null)
        {
            this.name = name;
            this.sprite = sprite;
            this.id = id;
            this.figmaName = figmaName;
        }
    }
}
