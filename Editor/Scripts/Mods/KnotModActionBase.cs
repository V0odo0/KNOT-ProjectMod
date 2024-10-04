using System;
using System.Collections;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    public abstract class KnotModActionBase : IKnotModAction, IKnotModDescriptor
    {
        internal const string BuiltinModActionPath = "Builtin Actions/";

        [SerializeField, HideInInspector] private byte _preventArrayElementStringNaming;

        public virtual string GetDescription() => string.Empty;

        public abstract IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed);
    }
}
