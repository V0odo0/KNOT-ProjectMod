using System;
using System.Collections;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor.PackageManager;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Add Package")]
    public class KnotAddPackageModAction : KnotModActionBase
    {
        public string Package;


        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            if (string.IsNullOrEmpty(Package))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed());
                yield break;
            }

            var request = Client.Add(Package);
            while (!request.IsCompleted)
                yield return null;

            onActionPerformed?.Invoke(this, new KnotModActionResult(request.Status == StatusCode.Success));
        }
    }
}
