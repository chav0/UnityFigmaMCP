using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal static class UnityObjectConverter
    {
        internal static UnityObject Convert(GameObject gameObject, IComponentMapper[] mappers, string path = null, bool includeChildren = true)
        {
            var gameObjectComponent = new GameObjectComponent
            {
                Layer = gameObject.layer,
                Tag = gameObject.tag
            };

            if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                if (prefabAsset != null)
                {
                    gameObjectComponent.PrefabPath = AssetDatabase.GetAssetPath(prefabAsset);
                    var component = FigmaComponentMap.GetOrCreate().FindComponent(null, prefabAsset.name);
                    if (component != null)
                        gameObjectComponent.ComponentKey = component.id;
                }
            }

            var result = new UnityObject
            {
                Name = gameObject.name,
                Path = path ?? gameObject.name,
                Active = gameObject.activeSelf,
                GameObject = gameObjectComponent
            };

            foreach (var mapper in mappers)
                mapper.ReadInto(gameObject, result);

            var button = gameObject.GetComponent<Button>();
            if (button != null)
            {
                result.Button = new ButtonComponent
                {
                    Interactable = button.interactable,
                    Transition = button.transition.ToString()
                };
            }

            var rectMask = gameObject.GetComponent<RectMask2D>();
            var mask = gameObject.GetComponent<Mask>();
            if (rectMask != null)
            {
                result.Mask = new MaskComponent { IsRectMask = true, ShowGraphic = true };
            }
            else if (mask != null)
            {
                result.Mask = new MaskComponent { IsRectMask = false, ShowGraphic = mask.showMaskGraphic };
            }

            if (includeChildren)
            {
                var transform = gameObject.transform;
                if (transform.childCount > 0)
                {
                    var children = new List<UnityObject>(transform.childCount);
                    for (var i = 0; i < transform.childCount; i++)
                    {
                        var child = transform.GetChild(i).gameObject;
                        var childPath = result.Path + "/" + child.name;
                        children.Add(Convert(child, mappers, childPath));
                    }
                    result.Children = children.ToArray();
                }
            }

            return result;
        }
    }
}
