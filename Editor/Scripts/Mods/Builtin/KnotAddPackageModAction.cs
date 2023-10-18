using System;
using System.Collections;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Add Package", MenuCustomName = BuiltinModActionPath + "Add Package", Order = -200)]
    public class KnotAddPackageModAction : KnotModActionBase
    {
        public string Package
        {
            get => _package;
            set => _package = value;
        }
        [SerializeField] private string _package = "com.package.name";


        public override string BuildDescription() => $"Add Package \"{Package}\"";

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
