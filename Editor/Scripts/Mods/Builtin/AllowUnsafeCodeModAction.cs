using System;
using System.Collections;
using System.Collections.Generic;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Allow Unsafe Code", MenuCustomName = BuiltinModActionPath + "Allow Unsafe Code")]
    public class AllowUnsafeCodeModAction : KnotModActionBase
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
