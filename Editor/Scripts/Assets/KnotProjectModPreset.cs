using System.Collections;
using System.Collections.Generic;
using Knot.Core;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [CreateAssetMenu(menuName = KnotProjectMod.CorePath + "Preset", fileName = "ProjectModPreset")]
    public class KnotProjectModPreset : ScriptableObject, IEnumerable<IKnotMod>
    {
        public List<IKnotMod> Mods => _mods ?? (_mods = new List<IKnotMod>());
        [SerializeReference]
        private List<IKnotMod> _mods;


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public IEnumerator<IKnotMod> GetEnumerator() => Mods.GetEnumerator();

        public IKnotMod[] BuildAllModsChain()
        {
            var mods = new List<IKnotMod>();
            var presetReferences = new HashSet<KnotProjectModPreset> { this };

            foreach (var m in Mods)
            {
                if (m is KnotPresetReferencesMod presetMod && presetMod.Presets != null)
                {
                    foreach (var preset in presetMod.Presets)
                    {
                        if (preset != null && !presetReferences.Contains(preset))
                        {
                            mods.AddRange(preset.BuildAllModsChain());
                            presetReferences.Add(preset);
                        }
                    }
                }
                else mods.Add(m);
            }

            return mods.ToArray();
        }
    }
}
