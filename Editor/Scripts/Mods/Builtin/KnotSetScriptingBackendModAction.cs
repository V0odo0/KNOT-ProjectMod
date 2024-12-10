using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knot.Core;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Set Scripting Backend", MenuCustomName = BuiltinModActionPath + "Set Scripting Backend")]
    public class KnotSetScriptingBackendModAction : KnotModActionBase, IKnotCombinedModAction
    {
        public BuildTargetGroup Target
        {
            get => _target;
            set => _target = value;
        }
        [SerializeField] private BuildTargetGroup _target = BuildTargetGroup.Standalone;

        public ScriptingImplementation Implementation
        {
            get => _implementation;
            set => _implementation = value;
        }
        [SerializeField] private ScriptingImplementation _implementation = ScriptingImplementation.IL2CPP;


        public override string GetDescription() => $"Set Scripting Backend for {Target} to {Implementation}";
        
        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            PlayerSettings.SetScriptingBackend(Target, Implementation);
            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
            yield break;
        }

        public IEnumerable<IKnotModAction> Combine(IEnumerable<IKnotCombinedModAction> orderedModActions)
        {
            return orderedModActions.TakeLast(1);
        }
    }
}
