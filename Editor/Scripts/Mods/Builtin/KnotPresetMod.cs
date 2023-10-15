using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    public class KnotPresetMod : IKnotProjectMod
    {
        public KnotProjectModPreset Preset
        {
            get => _preset;
            set => _preset = value;
        }
        [SerializeField] private KnotProjectModPreset _preset;
    }
}
