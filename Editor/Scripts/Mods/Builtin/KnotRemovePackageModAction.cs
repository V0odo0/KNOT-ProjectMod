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


        public override string GetDescription() => $"Remove package \"{Package}\"";


        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            if (string.IsNullOrEmpty(Package))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed($"{nameof(Package)}: {nameof(string.IsNullOrEmpty)}"));
                yield break;
            }
            
            var request = Client.Remove(Package);
            while (!request.IsCompleted)
                yield return null;

            onActionPerformed?.Invoke(this,
                request.Error?.errorCode is ErrorCode.NotFound
                    ? KnotModActionResult.Completed()
                    : KnotModActionResult.Failed(request.Error?.message));
        }
    }
}
