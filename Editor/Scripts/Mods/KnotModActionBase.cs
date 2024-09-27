using System;
using System.Collections;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    public abstract class KnotModActionBase : IKnotModAction
    {
        internal const string BuiltinModActionPath = "Builtin Actions/";

        [SerializeField, HideInInspector] private byte _preventArrayElementStringNaming;

        public virtual string BuildDescription() => GetType().Name;

        public abstract IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed);
    }
}
