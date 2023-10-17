using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(KnotProjectModPreset))]
    public class KnotProjectModPresetEditor : UnityEditor.Editor
    {
        private KnotProjectModPreset _target;

        private string _modActionDescription;
        [SerializeField] private bool _showDescriptionFoldout = true;


        void OnEnable()
        {
            _target = this.target as KnotProjectModPreset;
            RebuildModActionDescriptions();

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        void OnUndoRedoPerformed()
        {
            RebuildModActionDescriptions();
        }

        void RebuildModActionDescriptions()
        {
            _modActionDescription = string.Join("\n\n",
                _target.BuildAllModsChain().Where(m => m != null).OfType<IKnotModAction>()
                    .Select((action, i) => $"[{i + 1}] {action.BuildDescription()}"));
        }

        public override void OnInspectorGUI()
        {
            if (targets.Length > 1)
            {
                base.OnInspectorGUI();
                return;
            }

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
                RebuildModActionDescriptions();

            if (string.IsNullOrEmpty(_modActionDescription))
                return;

            EditorGUILayout.Space(10);
            _showDescriptionFoldout = EditorGUILayout.Foldout(_showDescriptionFoldout, "Actions");
            if (_showDescriptionFoldout)
            {
                EditorGUILayout.HelpBox(_modActionDescription, MessageType.None, true);
            }
        }
    }
}
