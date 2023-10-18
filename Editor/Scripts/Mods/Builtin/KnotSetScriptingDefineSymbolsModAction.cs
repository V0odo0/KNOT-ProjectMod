using System;
using System.Collections;
using System.Collections.Generic;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Set Scripting Define Symbols", MenuCustomName = BuiltinModActionPath + "Set Scripting Define Symbols")]
    public class KnotSetScriptingDefineSymbolsModAction : KnotModActionBase
    {
        public BuildTargetGroup Target
        {
            get => _target;
            set => _target = value;
        }
        [SerializeField] private BuildTargetGroup _target = BuildTargetGroup.Standalone;
        
        public List<string> Defines
        {
            get => _defines;
            set => _defines = value;
        }
        [SerializeField] private List<string> _defines = new List<string>();


        public override string BuildDescription() => $"Set Scripting Define Symbols \"{string.Join(", ", Defines)}\"";
        
        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            bool hasDiff = false;

            PlayerSettings.GetScriptingDefineSymbolsForGroup(Target, out var current);
            if (current != null)
            {
                if (current.Length == Defines.Count)
                {
                    foreach (var c in current)
                    {
                        if (!Defines.Contains(c))
                        {
                            hasDiff = true;
                            break;
                        }
                    }
                }
                else hasDiff = true;
            }

            if (hasDiff)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(Target, Defines.ToArray());

            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
            yield break;
        }
    }
}
