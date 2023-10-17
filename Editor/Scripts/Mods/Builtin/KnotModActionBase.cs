using System;
using System.Collections;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    public abstract class KnotModActionBase : IKnotModAction
    {
        public virtual bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }
        [SerializeField] protected bool _enabled = true;


        public virtual string BuildDescription() => GetType().Name;

        public abstract IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed);
    }
}
