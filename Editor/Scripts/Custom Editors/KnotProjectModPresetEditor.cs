using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [CustomEditor(typeof(KnotProjectModPreset))]
    public class KnotProjectModPresetEditor : UnityEditor.Editor
    {
        private KnotProjectModPreset _target;


        private void OnEnable()
        {
            _target = this.target as KnotProjectModPreset;
        }

        private void OnValidate()
        {

        }

    }
}
