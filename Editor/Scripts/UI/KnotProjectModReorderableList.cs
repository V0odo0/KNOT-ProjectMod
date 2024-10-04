using Knot.Core.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    public class KnotProjectModReorderableList : ReorderableList
    {
        internal static EditorExtensions.TypeInfo[] ModActionTypes { get; } =
            typeof(IKnotModAction).GetDerivedTypesInfo();

        public KnotProjectModReorderableList(SerializedObject serializedObject, SerializedProperty elements) : base(serializedObject, elements, true, true, true, true)
        {
            headerHeight  = 0;

            drawElementCallback = DrawElement;
            elementHeightCallback = ElementHeight;
            onAddDropdownCallback = OnAddDropdown;
            onRemoveCallback += OnRemove;
        }

        void OnRemove(ReorderableList reorderableList)
        {
            serializedProperty.DeleteArrayElementAtIndex(index);
            if (index > 0)
                index--;
            serializedProperty.serializedObject.ApplyModifiedProperties();
        }

        void DrawElement(Rect rect, int elementIndex, bool isactive, bool isfocused)
        {
            var property = serializedProperty.GetArrayElementAtIndex(elementIndex);

            EditorGUI.indentLevel = 1;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, property, new GUIContent(property.GetManagedReferenceTypeName()), property.isExpanded);
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }

        float ElementHeight(int elementIndex) =>
            EditorGUI.GetPropertyHeight(serializedProperty.GetArrayElementAtIndex(elementIndex)) + EditorGUIUtility.standardVerticalSpacing * 2;



        void OnAddDropdown(Rect rect, ReorderableList elementsList)
        {
            GenericMenu menu = new GenericMenu();
            foreach (var metadataType in ModActionTypes)
            {
                menu.AddItem(new GUIContent(metadataType.Info.MenuCustomName), false, () =>
                {
                    var instance = metadataType.GetInstance();
                    if (instance == null)
                        return;

                    serializedProperty.InsertArrayElementAtIndex(serializedProperty.arraySize);
                    serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1).managedReferenceValue =
                        instance;

                    index = serializedProperty.arraySize - 1;
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.DropDown(rect);
        }
    }
}
