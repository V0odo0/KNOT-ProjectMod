using System;
using System.Collections;
using System.Collections.Generic;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Set Scripting Define Symbols")]
    public class KnotSetScriptingDefineSymbolsModAction : KnotModActionBase
    {
        public BuildTargetGroup BuildTarget;
        public List<string> Defines;

        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            bool hasDiff = false;

            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTarget, out var current);
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
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTarget, Defines.ToArray());

            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
            yield break;
        }
    }
}
