using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityFigmaMCP.Editor
{
    [CreateAssetMenu(fileName = "FigmaAutoLayoutSettings", menuName = "UnityFigmaMCP/Settings")]
    public class FigmaAutoLayoutSettings : ScriptableObject
    {
        private const string DefaultAssetPath = "Assets/UnityFigmaMCP/Editor/FigmaAutoLayoutSettings.asset";
        private const string SearchFilter = "t:FigmaAutoLayoutSettings";

        [Header("Default folders")]
        [SerializeField] private DefaultAsset prefabFolder;
        [SerializeField] private DefaultAsset spritesFolder;
        
        [Header("Pipeline")]
        [SerializeField] private List<FigmaLayoutPipelineProfile> pipelines = new();
        
        [Header("Maps")]
        [SerializeField] private FigmaComponentMap componentMap;
        [SerializeField] private FigmaSpriteMap spriteMap;

        public string PrefabFolderPath => PrefabFolder != null ? AssetDatabase.GetAssetPath(prefabFolder) : "";
        public string SpritesFolderPath => SpritesFolder != null ? AssetDatabase.GetAssetPath(spritesFolder) : "";

        public DefaultAsset PrefabFolder
        {
            get => prefabFolder;
            set => prefabFolder = value;
        }

        public DefaultAsset SpritesFolder
        {
            get => spritesFolder;
            set => spritesFolder = value;
        }

        public List<FigmaLayoutPipelineProfile> Pipelines => pipelines;

        public FigmaLayoutPipelineProfile GetPipeline(string id)
        {
            var pipeline = !string.IsNullOrEmpty(id) 
                ? pipelines.FirstOrDefault(p => p.Id == id) 
                : null;

            if (pipeline == null)
            {
                pipeline = pipelines.FirstOrDefault();
            }

            return pipeline;
        }

        public string[] PipelineIds => pipelines.Select(p => p.Id).ToArray();

        public FigmaComponentMap ComponentMap
        {
            get
            {
                if (componentMap == null)
                    componentMap = FigmaComponentMap.GetOrCreate();
                return componentMap;
            }
        }

        public FigmaSpriteMap SpriteMap
        {
            get
            {
                if (spriteMap == null)
                    spriteMap = FigmaSpriteMap.GetOrCreate();
                return spriteMap;
            }
        }

        public static FigmaAutoLayoutSettings GetOrCreate()
        {
            var guids = AssetDatabase.FindAssets(SearchFilter);

            if (guids.Length > 0)
                return AssetDatabase.LoadAssetAtPath<FigmaAutoLayoutSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));

            if (!AssetDatabase.IsValidFolder("Assets/UnityFigmaMCP"))
                AssetDatabase.CreateFolder("Assets", "UnityFigmaMCP");

            if (!AssetDatabase.IsValidFolder("Assets/UnityFigmaMCP/Editor"))
                AssetDatabase.CreateFolder("Assets/UnityFigmaMCP", "Editor");

            var settings = CreateInstance<FigmaAutoLayoutSettings>();
            AssetDatabase.CreateAsset(settings, DefaultAssetPath);
            AssetDatabase.SaveAssets();

            return settings;
        }
    }
}
