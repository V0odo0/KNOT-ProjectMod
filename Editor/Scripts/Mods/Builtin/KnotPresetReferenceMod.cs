using System;
using Knot.ProjectMod.Editor.Attributes;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Preset Reference")]
    public class KnotPresetReferenceMod : IKnotMod
    {
        public KnotProjectModPreset Preset
        {
            get => _preset;
            set => _preset = value;
        }
        [SerializeField] private KnotProjectModPreset _preset;
    }
}
