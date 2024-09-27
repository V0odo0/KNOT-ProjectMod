using System;
using System.Collections;
using Knot.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Allow Unsafe Code", MenuCustomName = BuiltinModActionPath + "Allow Unsafe Code")]
    [MovedFrom(true, "Knot.ProjectMod.Editor", null, "AllowUnsafeCodeModAction")]
    public class KnotAllowUnsafeCodeModAction : KnotModActionBase
    {
        public bool AllowUnsafeCode
        {
            get => _allowUnsafeCode;
            set => _allowUnsafeCode = value;
        }
        [SerializeField] private bool _allowUnsafeCode;


        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            PlayerSettings.allowUnsafeCode = AllowUnsafeCode;
            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
            yield break;
        }
    }
}
