using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityFigmaMCP.Editor
{
    [CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
    internal sealed class SubclassSelectorDrawer : PropertyDrawer
    {
        private const string NoneLabel = "(none)";

        private static readonly Dictionary<Type, Type[]> CandidateCache = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);

            var dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            dropdownRect.xMin += EditorGUIUtility.labelWidth + 2f;

            var currentType = GetCurrentType(property);
            var caption = new GUIContent(currentType != null ? Prettify(currentType) : NoneLabel);

            if (EditorGUI.DropdownButton(dropdownRect, caption, FocusType.Keyboard))
                ShowTypeMenu(property, currentType);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, true);

        private static void ShowTypeMenu(SerializedProperty property, Type currentType)
        {
            var baseType = ResolveFieldType(property.managedReferenceFieldTypename);
            if (baseType == null)
                return;

            var serializedObject = property.serializedObject;
            var propertyPath = property.propertyPath;

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent(NoneLabel), currentType == null, () => Assign(serializedObject, propertyPath, null));

            foreach (var type in GetCandidates(baseType))
            {
                var captured = type;
                menu.AddItem(new GUIContent(Prettify(type)), currentType == type, () => Assign(serializedObject, propertyPath, Activator.CreateInstance(captured)));
            }

            menu.ShowAsContext();
        }

        private static void Assign(SerializedObject serializedObject, string propertyPath, object value)
        {
            serializedObject.Update();

            var property = serializedObject.FindProperty(propertyPath);
            if (property == null)
                return;

            property.managedReferenceValue = value;
            serializedObject.ApplyModifiedProperties();
        }

        private static Type GetCurrentType(SerializedProperty property)
        {
            var typename = property.managedReferenceFullTypename;
            return string.IsNullOrEmpty(typename) ? null : ResolveFieldType(typename);
        }

        private static Type ResolveFieldType(string managedReferenceTypename)
        {
            if (string.IsNullOrEmpty(managedReferenceTypename))
                return null;

            var parts = managedReferenceTypename.Split(' ');
            return parts.Length == 2 ? Type.GetType($"{parts[1]}, {parts[0]}") : null;
        }

        private static Type[] GetCandidates(Type baseType)
        {
            if (CandidateCache.TryGetValue(baseType, out var cached))
                return cached;

            return CandidateCache[baseType] = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetLoadableTypes)
                .Where(type => !type.IsAbstract && !type.IsInterface && baseType.IsAssignableFrom(type))
                .Where(type => type.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(type => type.Name)
                .ToArray();
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(type => type != null);
            }
        }

        private static string Prettify(Type type) => ObjectNames.NicifyVariableName(type.Name);
    }
}
