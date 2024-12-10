using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [CreateAssetMenu(menuName = KnotProjectMod.CorePath + "Preset", fileName = "ProjectModPreset")]
    public class KnotProjectModPreset : ScriptableObject, IEnumerable<IKnotMod>
    {
        public List<IKnotMod> Mods => _mods ?? (_mods = new List<IKnotMod>());
        [SerializeReference] private List<IKnotMod> _mods = new();


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IKnotMod> GetEnumerator() => Mods.GetEnumerator();

        public List<IKnotMod> BuildAllModsChain()
        {
            var mods = new List<IKnotMod>();
            var addedModPresets = new HashSet<KnotProjectModPreset> { this };
            foreach (var m in Mods)
            {
                if (m is KnotPresetReferencesMod { Presets: not null } presetMod)
                {
                    foreach (var preset in presetMod.Presets.Where(preset => preset != null && !addedModPresets.Contains(preset)))
                    {
                        mods.AddRange(preset.BuildAllModsChain());
                        addedModPresets.Add(preset);
                    }
                }
                else mods.Add(m);
            }

            return mods;
        }
    }
}
