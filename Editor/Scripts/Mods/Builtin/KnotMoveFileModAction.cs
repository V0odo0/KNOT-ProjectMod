using System;
using System.Collections;
using System.IO;
using Knot.ProjectMod.Editor.Attributes;
using UnityEditor;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Move File", MenuCustomName = BuiltinModActionPath + "Move File")]
    public class KnotMoveFileModAction : KnotModActionBase
    {
        public string Source
        {
            get => _source;
            set => _source = value;
        }
        [SerializeField] private string _source = "Assets/";

        public string Destination
        {
            get => _destination;
            set => _destination = value;
        }
        [SerializeField] private string _destination = "Assets/";


        public override string BuildDescription() => $"Move File from \"{Source}\" to \"{Destination}\"";
        
        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(Destination) || !File.Exists(Source))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed());
                yield break;
            }

            if (File.Exists(Destination))
            {
                onActionPerformed?.Invoke(this, new KnotModActionResult(true, $"{Destination} already exist"));
                yield break;
            }

            File.Move(Source, Destination);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
        }
    }
}
