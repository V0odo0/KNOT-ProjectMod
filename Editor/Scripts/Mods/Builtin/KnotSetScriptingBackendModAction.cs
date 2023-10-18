using System;
using System.Collections;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Set Scripting Backend", MenuCustomName = BuiltinModActionPath + "Set Scripting Backend")]
    public class KnotSetScriptingBackendModAction : KnotModActionBase
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


        public override string BuildDescription() => $"Set Scripting Backend for {Target} to {Implementation}";
        
        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            PlayerSettings.SetScriptingBackend(Target, Implementation);
            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
            yield break;
        }
    }
}
