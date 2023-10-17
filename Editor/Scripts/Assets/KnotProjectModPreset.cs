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
            List<IKnotMod> mods = new List<IKnotMod>();

            foreach (var m in Mods)
            {
                if (m is KnotPresetReferenceMod presetMod && presetMod.Preset != null)
                    mods.AddRange(presetMod.Preset.BuildAllModsChain());
                else mods.Add(m);
            }

            return mods.ToArray();
        }

        [ContextMenu(nameof(PerformActions))]
        public void PerformActions()
        {
            KnotProjectMod.Start(this);
        }
    }
}
