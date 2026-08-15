using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityFigmaMCP.Common;
using Object = UnityEngine.Object;

namespace UnityFigmaMCP.Editor.Exporters
{
    public class PrefabExporter
    {
        private readonly FigmaComponentMap _componentMap;
        private readonly FigmaSpriteMap _spriteMap;
        private readonly FigmaFile _file;
        private readonly FigmaLayoutPipelineProfile _pipelineProfile;
        private Scene _previewScene;

        private LayerMask UILayer => LayerMask.NameToLayer("UI");

        public PrefabExporter(FigmaAutoLayoutSettings settings, FigmaFile file, FigmaLayoutPipelineProfile profile)
        {
            _componentMap = settings.ComponentMap;
            _spriteMap = settings.SpriteMap;
            _file = file;
            _pipelineProfile = profile;
        }

        public GameObject Export(string prefabName, string prefabsPath)
        {
            _componentMap.Clean();
            _previewScene = EditorSceneManager.NewPreviewScene();

            try
            {
                var frame = _file.root;

                switch (frame.type)
                {
                    case FigmaObjectType.COMPONENT_SET:
                        return CreateVariants(frame, prefabsPath);
                    case FigmaObjectType.FRAME:
                        return CreatePrefab(frame, prefabName, prefabsPath);
                    case FigmaObjectType.COMPONENT:
                    case FigmaObjectType.INSTANCE:
                        var prefab = CreatePrefab(frame, prefabName, prefabsPath);
                        RegisterComponent(frame, prefabName, prefab);
                        return prefab;
                    default:
                        throw new Exception("Unsupported frame type. Select a Frame, Component, or Component Set.");
                }
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(_previewScene);
            }
        }

        private GameObject CreatePrefab(FigmaObject frame, string prefabName, string prefabsPath)
        {
            var rootObject = CreateInPreview(frame.name);

            ApplyPipeline(rootObject, frame, null, frame);

            if (frame.children != null)
            {
                foreach (var child in frame.children)
                    CreateGameObject(child, frame, rootObject.GetComponent<RectTransform>(), prefabsPath);
            }

            return SavePrefab(rootObject, prefabName, prefabsPath);
        }

        private GameObject CreateVariants(FigmaObject componentSet, string prefabsPath)
        {
            if (componentSet.children == null || componentSet.children.Length == 0)
                return null;

            var componentName = FigmaAssetPathHelper.SanitizeName(componentSet.name);
            var componentKey = _file.GetComponentKey(componentSet.id) ?? componentSet.id;
            var originPrefab = CreatePrefab(componentSet.children[0], componentName, prefabsPath);

            _componentMap.AddComponent(componentKey, componentName, originPrefab);

            foreach (var child in componentSet.children)
                CreateVariant(componentKey, componentName, originPrefab, child, prefabsPath);

            return originPrefab;
        }

        private void CreateVariant(string componentKey, string componentName,
            GameObject originPrefab, FigmaObject variantFigmaObject, string prefabsPath)
        {
            var variantName = FigmaAssetPathHelper.ExtractVariantPrefabName(componentName, variantFigmaObject.name);
            var variantKey = _file.GetComponentKey(variantFigmaObject.id) ?? variantFigmaObject.id;

            var existingPrefab = _componentMap.FindPrefab(variantKey, variantName);
            if (existingPrefab == null)
            {
                var sourceObject = InstantiateInPreview(originPrefab);
                ApplyVariant(sourceObject, variantFigmaObject);
                existingPrefab = SavePrefab(sourceObject, variantName, prefabsPath);
            }

            _componentMap.AddVariant(componentKey, componentName, variantKey, variantName, existingPrefab);
        }

        private void ApplyVariant(GameObject instance, FigmaObject variantRoot)
        {
            ApplyPipeline(instance, variantRoot, instance.transform.parent, variantRoot);
            SyncVariantChildren(instance.transform, variantRoot, variantRoot);
        }

        private void SyncVariantChildren(Transform parent, FigmaObject figmaParent, FigmaObject rootFrame)
        {
            var figmaChildren = figmaParent.children;

            if (figmaChildren == null || figmaChildren.Length == 0)
            {
                for (var i = 0; i < parent.childCount; i++)
                    parent.GetChild(i).gameObject.SetActive(false);

                return;
            }

            foreach (var figmaChild in figmaChildren)
            {
                var existingChild = parent.Find(figmaChild.name);
                if (existingChild != null)
                {
                    existingChild.gameObject.SetActive(figmaChild.visible);
                    ApplyPipeline(existingChild.gameObject, figmaChild, parent, rootFrame, figmaParent);
                    if (!IsCollapsedInstance(figmaChild))
                        SyncVariantChildren(existingChild, figmaChild, rootFrame);
                }
                else
                {
                    CreateVariantChild(figmaChild, rootFrame, parent);
                }
            }

            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (figmaChildren.All(f => f.name != child.name))
                    child.gameObject.SetActive(false);
            }
        }

        private void CreateVariantChild(FigmaObject figmaObject, FigmaObject rootFrame, Transform parent)
        {
            var childObject = CreateInPreview(figmaObject.name);

            childObject.SetActive(figmaObject.visible);
            ApplyPipeline(childObject, figmaObject, parent, rootFrame);
            GameObjectUtility.EnsureUniqueNameForSibling(childObject);

            if (figmaObject.children == null)
                return;

            foreach (var child in figmaObject.children)
                CreateVariantChild(child, rootFrame, childObject.GetComponent<RectTransform>());
        }

        private void CreateGameObject(FigmaObject figmaObject, FigmaObject rootFrame, Transform parent, string prefabsPath)
        {
            GameObject childObject;

            if (figmaObject.type == FigmaObjectType.INSTANCE)
            {
                childObject = InstantiatePrefab(figmaObject, prefabsPath);
                if (childObject != null)
                {
                    new RectTransformPipelineStep()
                        .Execute(new ObjectLayoutContext(childObject, figmaObject, parent, rootFrame, _spriteMap));
                    GameObjectUtility.EnsureUniqueNameForSibling(childObject);
                    return;
                }
            }

            childObject = CreateInPreview(figmaObject.name);

            childObject.SetActive(figmaObject.visible);

            if (figmaObject.type != FigmaObjectType.DOCUMENT && figmaObject.type != FigmaObjectType.CANVAS)
                ApplyPipeline(childObject, figmaObject, parent, rootFrame);

            GameObjectUtility.EnsureUniqueNameForSibling(childObject);

            if (figmaObject.children == null)
                return;

            foreach (var child in figmaObject.children)
            {
                CreateGameObject(child, rootFrame, childObject.GetComponent<RectTransform>(), prefabsPath);
            }

            if (figmaObject.type == FigmaObjectType.COMPONENT || figmaObject.type == FigmaObjectType.INSTANCE)
            {
                var rectTransform = childObject.GetComponent<RectTransform>();
                var position = rectTransform.anchoredPosition;
                var size = rectTransform.sizeDelta;

                var prefabName = FigmaAssetPathHelper.SanitizeName(figmaObject.name);
                var prefab = SavePrefab(childObject, prefabName, prefabsPath);
                if (prefab == null)
                    return;

                var instance = InstantiateInPreview(prefab);
                instance.transform.SetParent(parent);
                GameObjectUtility.EnsureUniqueNameForSibling(instance);

                rectTransform = instance.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = position;
                rectTransform.sizeDelta = size;

                PrefabUtility.RecordPrefabInstancePropertyModifications(instance.GetComponent<Transform>());

                RegisterComponent(figmaObject, prefabName, prefab);
            }
        }

        private void ApplyPipeline(GameObject gameObject, FigmaObject figmaObject, Transform parent,
            FigmaObject rootFrame, FigmaObject parentFigmaObject = null)
        {
            var layoutContext = new ObjectLayoutContext(gameObject, figmaObject, parent, rootFrame, _spriteMap, parentFigmaObject);

            foreach (var step in _pipelineProfile.PipelineSteps)
                step?.Execute(layoutContext);
        }

        private void RegisterComponent(FigmaObject figmaObject, string prefabName, GameObject prefab)
        {
            if (figmaObject.type == FigmaObjectType.COMPONENT)
            {
                var componentKey = _file.GetComponentKey(figmaObject.id) ?? figmaObject.id;
                _componentMap.AddComponent(componentKey, prefabName, prefab);
                return;
            }

            var componentSetId = _file.GetComponentSetId(figmaObject.componentId);
            if (!string.IsNullOrEmpty(componentSetId))
            {
                var componentSetKey = _file.GetComponentKey(componentSetId) ?? componentSetId;
                var variantKey = _file.GetComponentKey(figmaObject.componentId) ?? figmaObject.componentId;
                _componentMap.AddVariant(componentSetKey, null, variantKey, prefabName, prefab);
            }
            else
            {
                var componentKey = _file.GetComponentKey(figmaObject.componentId) ?? figmaObject.componentId;
                _componentMap.AddComponent(componentKey, prefabName, prefab);
            }
        }

        private bool IsCollapsedInstance(FigmaObject figmaObject)
        {
            if (figmaObject.type != FigmaObjectType.INSTANCE)
                return false;

            var componentName = FigmaAssetPathHelper.SanitizeName(figmaObject.name);
            var componentKey = _file.GetComponentKey(figmaObject.componentId) ?? figmaObject.componentId;

            if (_componentMap.FindPrefab(componentKey, componentName) != null)
                return true;

            return _spriteMap.Find(componentKey) != null || _spriteMap.Find(figmaObject.name) != null;
        }

        private GameObject InstantiatePrefab(FigmaObject figmaObject, string prefabsPath)
        {
            var componentName = FigmaAssetPathHelper.SanitizeName(figmaObject.name);
            var componentKey = _file.GetComponentKey(figmaObject.componentId) ?? figmaObject.componentId;

            var prefab = _componentMap.FindPrefab(componentKey, componentName);
            if (prefab != null)
                return InstantiateInPreview(prefab);

            var path = prefabsPath.TrimEnd('/');
            var guids = AssetDatabase.FindAssets($"t:Prefab {componentName}", new[] {path});
            foreach (var guid in guids)
            {
                var found = (GameObject) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject));
                if (found != null && found.name == componentName)
                    return InstantiateInPreview(found);
            }

            var sprite = _spriteMap.Find(componentKey) ?? _spriteMap.Find(figmaObject.name);
            if (sprite != null)
                return CreateSpriteObject(figmaObject.name, sprite);

            return null;
        }

        private GameObject CreateSpriteObject(string objectName, Sprite sprite)
        {
            var spriteObject = new GameObject(objectName, typeof(RectTransform), typeof(Image))
            {
                layer = UILayer
            };

            SceneManager.MoveGameObjectToScene(spriteObject, _previewScene);

            var image = spriteObject.GetComponent<Image>();

            image.sprite = sprite;
            image.type = sprite.border != Vector4.zero ? Image.Type.Sliced : Image.Type.Simple;
            image.raycastTarget = false;

            return spriteObject;
        }

        private GameObject CreateInPreview(string objectName)
        {
            var obj = new GameObject
            {
                name = objectName,
                layer = UILayer
            };

            SceneManager.MoveGameObjectToScene(obj, _previewScene);
            return obj;
        }

        private GameObject InstantiateInPreview(GameObject prefab)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            SceneManager.MoveGameObjectToScene(instance, _previewScene);
            PrefabUtility.RecordPrefabInstancePropertyModifications(instance.GetComponent<Transform>());
            return instance;
        }

        private GameObject SavePrefab(GameObject gameObject, string prefabName, string prefabsPath)
        {
            var assetPath = FigmaAssetPathHelper.BuildAssetPath(prefabsPath, prefabName, "prefab");
            var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            var prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, uniqueAssetPath);

            Object.DestroyImmediate(gameObject);

            return prefab;
        }
    }
}
