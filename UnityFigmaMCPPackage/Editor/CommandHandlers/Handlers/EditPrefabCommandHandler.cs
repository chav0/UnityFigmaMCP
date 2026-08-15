using System;
using UnityEngine;
using UnityFigmaMCP.Common;
using Object = UnityEngine.Object;

namespace UnityFigmaMCP.Editor
{
    internal sealed class EditPrefabCommandHandler : ICommandHandler<EditPrefabCommand, EditPrefabCommandResult>
    {
        public EditPrefabCommandResult Handle(ICommandContext context, EditPrefabCommand command)
        {
            if (command.Edits == null || command.Edits.Length == 0)
                throw new Exception("No edits supplied.");

            var mappers = context.Mappers;
            using var scope = PrefabEditScope.Create(command.PrefabPath);

            for (var i = 0; i < command.Edits.Length; i++)
            {
                var edit = command.Edits[i];

                try
                {
                    Apply(scope, edit, mappers);
                }
                catch (Exception exception)
                {
                    throw new Exception(
                        $"Edit {i} (\"{edit?.Op}\" on \"{Describe(edit?.Path)}\") failed: {exception.Message} " +
                        "Nothing was saved — the prefab is unchanged.",
                        exception);
                }
            }

            scope.Save();

            return new EditPrefabCommandResult
            {
                Applied = command.Edits.Length,
                Root = UnityObjectConverter.Convert(scope.Root, mappers.All, null, command.IncludeChildren)
            };
        }

        private static void Apply(PrefabEditScope scope, PrefabEdit edit, ComponentMappers mappers)
        {
            if (edit == null)
                throw new Exception("Edit entry is null.");

            switch (edit.Op)
            {
                case PrefabEditOps.Create:
                    Create(scope, edit);
                    return;

                case PrefabEditOps.Delete:
                    Delete(scope, edit);
                    return;

                case PrefabEditOps.Reparent:
                    Reparent(scope, edit);
                    return;

                case PrefabEditOps.SetActive:
                    if (!edit.Active.HasValue)
                        throw new Exception("\"setActive\" requires Active.");
                    
                    Resolve(scope, edit.Path).SetActive(edit.Active.Value);
                    return;

                case PrefabEditOps.RectTransform:
                    ApplyOrRemove(mappers.RectTransform, Resolve(scope, edit.Path), edit.RectTransform, edit.Remove);
                    return;

                case PrefabEditOps.Image:
                    ApplyOrRemove(mappers.Image, Resolve(scope, edit.Path), edit.Image, edit.Remove);
                    return;

                case PrefabEditOps.Text:
                    ApplyOrRemove(mappers.Text, Resolve(scope, edit.Path), edit.Text, edit.Remove);
                    return;

                case PrefabEditOps.HorizontalLayout:
                    ApplyOrRemove(mappers.HorizontalLayout, Resolve(scope, edit.Path), edit.HorizontalLayout, edit.Remove);
                    return;

                case PrefabEditOps.VerticalLayout:
                    ApplyOrRemove(mappers.VerticalLayout, Resolve(scope, edit.Path), edit.VerticalLayout, edit.Remove);
                    return;

                case PrefabEditOps.GridLayout:
                    ApplyOrRemove(mappers.GridLayout, Resolve(scope, edit.Path), edit.GridLayout, edit.Remove);
                    return;

                case PrefabEditOps.ContentSizeFitter:
                    ApplyOrRemove(mappers.ContentSizeFitter, Resolve(scope, edit.Path), edit.ContentSizeFitter, edit.Remove);
                    return;

                default:
                    throw new Exception($"Unknown op \"{edit.Op}\".");
            }
        }

        private static void ApplyOrRemove<TUnity, TDto>(
            ComponentMapper<TUnity, TDto> mapper, GameObject target, TDto payload, bool remove)
            where TUnity : Component
        {
            if (remove)
            {
                mapper.Remove(target);
                return;
            }

            if (payload == null)
                throw new Exception("The payload field for this op is required unless Remove is true.");

            mapper.Apply(target, payload);
        }

        private static void Create(PrefabEditScope scope, PrefabEdit edit)
        {
            if (string.IsNullOrEmpty(edit.Name))
                throw new Exception("\"create\" requires Name.");

            var parent = Resolve(scope, edit.Path);

            var created = new GameObject(edit.Name);
            created.AddComponent<RectTransform>();
            created.transform.SetParent(parent.transform, false);
        }

        private static void Delete(PrefabEditScope scope, PrefabEdit edit)
        {
            var target = Resolve(scope, edit.Path);

            if (target == scope.Root)
                throw new Exception("Cannot delete the prefab root.");

            Object.DestroyImmediate(target);
        }

        private static void Reparent(PrefabEditScope scope, PrefabEdit edit)
        {
            if (edit.NewParentPath == null)
                throw new Exception("\"reparent\" requires NewParentPath (empty string moves the object to the root).");

            var target = Resolve(scope, edit.Path);

            if (target == scope.Root)
                throw new Exception("Cannot reparent the prefab root.");

            var newParent = scope.FindInContext(edit.NewParentPath);
            if (newParent == null)
                throw new Exception($"New parent not found at \"{Describe(edit.NewParentPath)}\".");

            target.transform.SetParent(newParent.transform, false);

            if (edit.SiblingIndex.HasValue)
                target.transform.SetSiblingIndex(edit.SiblingIndex.Value);
        }

        private static GameObject Resolve(PrefabEditScope scope, string path)
        {
            var target = scope.FindInContext(path);

            if (target == null)
                throw new Exception($"Object not found at \"{Describe(path)}\".");

            return target;
        }

        private static string Describe(string path) => string.IsNullOrEmpty(path) ? "<root>" : path;
    }
}
