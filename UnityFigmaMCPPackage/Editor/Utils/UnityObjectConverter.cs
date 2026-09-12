using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal static class UnityObjectConverter
    {
        internal static UnityObject Convert(GameObject gameObject, IComponentMapper[] mappers, string path = null, bool includeChildren = true, bool isRoot = true)
        {
            GameObjectComponent go = null;
            var isNestedPrefab = false;

            if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                if (prefabAsset != null)
                {
                    go = new GameObjectComponent { Prefab = AssetDatabase.GetAssetPath(prefabAsset) };
                    var component = FigmaComponentMap.GetOrCreate().FindComponent(null, prefabAsset.name);
                    if (component != null)
                        go.Key = component.id;

                    isNestedPrefab = !isRoot;
                }
            }

            var currentPath = path ?? gameObject.name;

            var result = new UnityObject
            {
                Name = gameObject.name,
                Path = path,
                Active = gameObject.activeSelf ? null : (bool?)false,
                GO = go
            };

            foreach (var mapper in mappers)
                mapper.ReadInto(gameObject, result);

            var button = gameObject.GetComponent<Button>();
            if (button != null)
            {
                result.Btn = new ButtonComponent
                {
                    Interactable = button.interactable ? null : (bool?)false,
                    Transition = button.transition.ToString()
                };
            }

            var rectMask = gameObject.GetComponent<RectMask2D>();
            var mask = gameObject.GetComponent<Mask>();
            if (rectMask != null)
            {
                result.Mask = new MaskComponent { IsRect = true };
            }
            else if (mask != null)
            {
                result.Mask = new MaskComponent
                {
                    IsRect = false,
                    Show = mask.showMaskGraphic ? null : (bool?)false
                };
            }

            if (includeChildren && !isNestedPrefab)
            {
                var transform = gameObject.transform;
                if (transform.childCount > 0)
                {
                    var children = new List<UnityObject>(transform.childCount);
                    for (var i = 0; i < transform.childCount; i++)
                    {
                        var child = transform.GetChild(i).gameObject;
                        var childPath = currentPath + "/" + child.name;
                        children.Add(Convert(child, mappers, childPath, true, false));
                    }
                    result.Children = children.ToArray();
                }
            }

            return result;
        }
    }
}
