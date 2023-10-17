using System;
using System.Collections;
using System.Threading.Tasks;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.Scripting.APIUpdating;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName:"Remove Package")]
    public class KnotRemovePackageModAction : KnotModActionBase
    {
        public string Package;


        public override string BuildDescription() => $"Remove Package \"{Package}\"";
        
        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            if (string.IsNullOrEmpty(Package))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed());
                yield break;
            }

            var request = Client.Remove(Package);
            while (!request.IsCompleted)
                yield return null;

            onActionPerformed?.Invoke(this, new KnotModActionResult(request.Status == StatusCode.Success));
        }
    }
}
