using System;
using System.Collections;
using System.IO;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Move Directory")]
    public class KnotMoveDirectoryModAction : KnotModActionBase
    {
        public string Source;
        public string Destination;


        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(Destination) || !Directory.Exists(Source))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed());
                yield break;
            }

            if (Directory.Exists(Destination) && Directory.GetFiles(Destination).Length == 0 && Directory.GetDirectories(Destination).Length == 0)
                Directory.Delete(Destination);
            else if (Directory.Exists(Destination))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed());
                yield break;
            }

            Directory.Move(Source, Destination);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
        }
    }
}
