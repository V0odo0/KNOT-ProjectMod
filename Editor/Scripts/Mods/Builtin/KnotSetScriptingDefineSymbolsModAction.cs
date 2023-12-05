using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public ActionMethod Action
        {
            get => _action;
            set => _action = value;
        }
        [SerializeField] private ActionMethod _action = ActionMethod.Override;

        public List<string> Defines
        {
            get => _defines;
            set => _defines = value;
        }
        [SerializeField] private List<string> _defines = new List<string>();


        public override string BuildDescription() => $"{Action} Scripting Define Symbols \"{string.Join(", ", Defines)}\"";
        
        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            bool hasDiff = false;

            var newDefines = new List<string>();

            PlayerSettings.GetScriptingDefineSymbolsForGroup(Target, out var curDefines);
            if (curDefines != null)
            {
                switch (Action)
                {
                    case ActionMethod.Override:
                        newDefines.AddRange(Defines);
                        if (curDefines.Length == newDefines.Count)
                        {
                            foreach (var c in curDefines)
                            {
                                if (!newDefines.Contains(c))
                                {
                                    hasDiff = true;
                                    break;
                                }
                            }
                        }
                        else hasDiff = true;
                        break;
                    case ActionMethod.Append:
                        newDefines.AddRange(curDefines);
                        foreach (var d in Defines)
                        {
                            if (!newDefines.Contains(d))
                            {
                                newDefines.Add(d);
                                hasDiff = true;
                            }
                        }
                        break;
                    case ActionMethod.Remove:
                        newDefines.AddRange(curDefines);
                        foreach (var d in Defines)
                        {
                            if (newDefines.Contains(d))
                            {
                                newDefines.Remove(d);
                                hasDiff = true;
                            }
                        }
                        break;
                }
            }

            if (hasDiff)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(Target, newDefines.ToArray());
            
            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
            yield break;
        }

        [Serializable]
        public enum ActionMethod
        {
            Override,
            Append,
            Remove
        }
    }
}
