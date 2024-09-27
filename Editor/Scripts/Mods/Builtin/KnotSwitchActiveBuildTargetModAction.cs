using System;
using System.Collections;
using Knot.Core;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Switch Active Build Target", MenuCustomName = BuiltinModActionPath + "Switch Active Build Target")]
    public class KnotSwitchActiveBuildTargetModAction : KnotModActionBase
    {
        public BuildTargetGroup TargetGroup
        {
            get => _targetGroup;
            set => _targetGroup = value;
        }
        [SerializeField] private BuildTargetGroup _targetGroup = BuildTargetGroup.Standalone;


        public BuildTarget Target
        {
            get => _target;
            set => _target = value;
        }
        [SerializeField] private BuildTarget _target = BuildTarget.StandaloneWindows64;


        public override string BuildDescription() => $"Switch Active Build Target to {TargetGroup} / {Target}";
        
        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            var switchBuildTarget = EditorUserBuildSettings.SwitchActiveBuildTarget(TargetGroup, Target);
            onActionPerformed?.Invoke(this, new KnotModActionResult(switchBuildTarget));
            yield break;
        }
    }
}
