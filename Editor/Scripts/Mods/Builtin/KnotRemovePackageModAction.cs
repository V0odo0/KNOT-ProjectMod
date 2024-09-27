using System;
using System.Collections;
using Knot.Core;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName:"Remove Package", MenuCustomName = BuiltinModActionPath + "Remove Package", Order = -100)]
    public class KnotRemovePackageModAction : KnotModActionBase
    {
        public string Package
        {
            get => _package;
            set => _package = value;
        }
        [SerializeField] private string _package = "com.package.name";


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
