using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Knot.ProjectMod.Editor
{
    public static class KnotEditorExtensions
    {
        private static readonly Dictionary<Type, string> _managedReferenceTypeNamesCache = new Dictionary<Type, string>();
        private static readonly Dictionary<Type, TypeInfo[]> _derivedTypesInfoCache = new Dictionary<Type, TypeInfo[]>();
        private static Dictionary<string, Texture> _cachedIcons = new Dictionary<string, Texture>();
        

        internal static TypeInfo[] GetDerivedTypesInfo(this Type baseType)
        {
            if (_derivedTypesInfoCache.ContainsKey(baseType) && _derivedTypesInfoCache[baseType] != null)
                return _derivedTypesInfoCache[baseType];

            bool IsValidType(Type t)
            {
                //Non abstract
                return !t.IsAbstract &&
                       //Non generic 
                       !t.IsGenericType &&
                       //Not interface
                       !t.IsInterface &&
                       //Not derived from Unity Object
                       !t.IsSubclassOf(typeof(UnityEngine.Object)) &&
                       //Has default constructor
                       t.GetConstructors().Any(c => c.GetParameters().Length == 0);
            }

            List<TypeInfo> derivedTypes = new List<TypeInfo>();

            foreach (var type in TypeCache.GetTypesDerivedFrom(baseType).Where(IsValidType))
                derivedTypes.Add(new TypeInfo(type));

            _derivedTypesInfoCache.Add(baseType, derivedTypes.OrderBy(t => t.Info.Order).ToArray());

            return _derivedTypesInfoCache[baseType];
        }

        internal static Type GetManagedReferenceType(this SerializedProperty property)
        {
            var parts = property.managedReferenceFullTypename.Split(' ');
            if (parts.Length == 2)
            {
                var assemblyPart = parts[0];
                var nsClassnamePart = parts[1];
                return Type.GetType($"{nsClassnamePart}, {assemblyPart}");
            }

            return null;
        }

        internal static string GetManagedReferenceTypeName(this SerializedProperty property)
        {
            var type = property.GetManagedReferenceType();
            if (type == null)
                return property.displayName;

            if (_managedReferenceTypeNamesCache.ContainsKey(type))
                return _managedReferenceTypeNamesCache[type];

            var name = type.GetCustomAttribute<KnotTypeInfoAttribute>()?.DisplayName
                       ?? ObjectNames.NicifyVariableName(type.Name);
            _managedReferenceTypeNamesCache.Add(type, name);

            return name;
        }

        internal static float GetChildPropertiesHeight(this SerializedProperty property,
            params string[] exceptPropertyPaths)
        {
            float h = 0;
            foreach (SerializedProperty childProperty in property)
            {
                if (exceptPropertyPaths.Contains(childProperty.propertyPath))
                    continue;

                h += EditorGUI.GetPropertyHeight(childProperty, childProperty.isExpanded) + EditorGUIUtility.standardVerticalSpacing;
            }

            return h;
        }

        internal static void DrawChildProperties(this SerializedProperty property, Rect position,
            params string[] exceptPropertyPaths)
        {
            if (!property.hasChildren)
                return;

            EditorGUI.BeginChangeCheck();
            foreach (SerializedProperty childProperty in property)
            {
                if (exceptPropertyPaths.Contains(childProperty.propertyPath))
                    continue;

                EditorGUI.PropertyField(position, childProperty, true);
                position.y += EditorGUI.GetPropertyHeight(childProperty, childProperty.isExpanded) + EditorGUIUtility.standardVerticalSpacing;
            }

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }

        internal static SerializedProperty FindParentProperty(this SerializedProperty serializedProperty)
        {
            var propertyPaths = serializedProperty.propertyPath.Split('.');
            if (propertyPaths.Length <= 1)
                return default;

            var parentSerializedProperty = serializedProperty.serializedObject.FindProperty(propertyPaths.First());
            for (var index = 1; index < propertyPaths.Length - 1; index++)
            {
                if (propertyPaths[index] == "Array")
                {
                    if (index + 1 == propertyPaths.Length - 1)
                        break;

                    if (propertyPaths.Length > index + 1 && Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
                    {
                        var match = Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                        var arrayIndex = int.Parse(match.Groups[1].Value);
                        parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                        index++;
                    }
                }
                else parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
            }

            return parentSerializedProperty;
        }

        internal static VisualElement GetFallbackPropertyGUI(this SerializedProperty property)
        {
            IMGUIContainer container = new IMGUIContainer(() => { EditorGUILayout.PropertyField(property); });
            container.style.marginLeft = container.style.marginRight = 3;
            container.style.marginTop = container.style.marginBottom = 1;

            return container;
        }

        public static Texture GetIcon(string iconName)
        {
            if (_cachedIcons.ContainsKey(iconName))
                return _cachedIcons[iconName];

            Debug.unityLogger.logEnabled = false;
            Texture icon = EditorGUIUtility.IconContent(iconName)?.image;
            Debug.unityLogger.logEnabled = true;

            if (icon == null)
                icon = Resources.Load<Texture>(iconName);

            if (icon == null)
                return null;

            if (!_cachedIcons.ContainsKey(iconName))
                _cachedIcons.Add(iconName, icon);

            return icon;
        }

        public class TypeInfo
        {
            public readonly Type Type;
            public readonly KnotTypeInfoAttribute Info;
            public readonly GUIContent Content;


            public TypeInfo(Type type)
            {
                Type = type;
                Info = type.GetCustomAttribute<KnotTypeInfoAttribute>() ??
                       new KnotTypeInfoAttribute(ObjectNames.NicifyVariableName(type.Name));

                Content = new GUIContent(Info.DisplayName, GetIcon(Info.IconName));
            }

            public object GetInstance() => Activator.CreateInstance(Type);
        }
    }
}
