using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(KnotProjectModPreset))]
    public class KnotProjectModPresetEditor : UnityEditor.Editor
    {
        private KnotProjectModPreset _target;

        private KnotProjectModReorderableList _modReorderableList;


        void OnEnable()
        {
            _target = target as KnotProjectModPreset;

            _modReorderableList =
                new KnotProjectModReorderableList(serializedObject, serializedObject.FindProperty("_mods"));
        }

        public override void OnInspectorGUI()
        {
            if (targets.Length > 1)
            {
                base.OnInspectorGUI();
                return;
            }

            
            _modReorderableList.DoLayoutList();

            EditorGUILayout.Space(10);

            GUI.enabled = _target.Mods.Any();
            if (GUILayout.Button("Run", EditorStyles.miniButtonMid))
            {
                if (EditorUtility.DisplayDialog(KnotProjectMod.CoreName,
                        $"Some actions may lead to unpredictable results. Continue anyways?", "Yes", "No"))
                {
                    KnotProjectMod.Start(_target);
                }
            }
            GUI.enabled = true;
        }
    }
}
