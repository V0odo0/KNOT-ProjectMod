using System;
using System.Collections.Generic;
using Knot.Core;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Preset Reference")]
    public class KnotPresetReferencesMod : IKnotMod
    {
        public List<KnotProjectModPreset> Presets
        {
            get => _presets;
            set => _presets = value;
        }
        [SerializeField] private List<KnotProjectModPreset> _presets;
    }
}
