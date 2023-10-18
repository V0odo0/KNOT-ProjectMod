using System.Collections;
using System.Collections.Generic;
using Knot.ProjectMod.Editor.Attributes;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [CreateAssetMenu(menuName = KnotProjectMod.CorePath + "Preset", fileName = "ProjectModPreset")]
    public class KnotProjectModPreset : ScriptableObject, IEnumerable<IKnotMod>
    {
        public List<IKnotMod> Mods => _mods ?? (_mods = new List<IKnotMod>());
        [SerializeReference, KnotTypePicker(typeof(IKnotMod))]
        private List<IKnotMod> _mods;


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public IEnumerator<IKnotMod> GetEnumerator() => Mods.GetEnumerator();

        public IKnotMod[] BuildAllModsChain()
        {
            var mods = new List<IKnotMod>();
            var presetReferences = new HashSet<KnotProjectModPreset> { this };

            foreach (var m in Mods)
            {
                if (m is KnotPresetReferenceMod presetMod && 
                    presetMod.Preset != null &&
                    !presetReferences.Contains(presetMod.Preset))
                {
                    mods.AddRange(presetMod.Preset.BuildAllModsChain());
                    presetReferences.Add(presetMod.Preset);
                }
                else mods.Add(m);
            }

            return mods.ToArray();
        }
    }
}
