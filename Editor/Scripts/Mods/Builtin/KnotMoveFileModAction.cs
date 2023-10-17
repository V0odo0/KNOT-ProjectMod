using System;
using System.Collections;
using System.IO;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Move File")]
    public class KnotMoveFileModAction : KnotModActionBase
    {
        public string Source;
        public string Destination;


        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(Destination) || !File.Exists(Source))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed());
                yield break;
            }

            File.Move(Source, Destination);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
        }
    }
}
