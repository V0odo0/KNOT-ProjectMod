using System;
using System.Collections;
using System.IO;
using Knot.Core;
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


        public override string GetDescription() => $"Move File from \"{Source}\" to \"{Destination}\"";
        
        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(Destination))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed($"{nameof(Source)} or {nameof(Destination)}: {nameof(string.IsNullOrEmpty)}"));
                yield break;
            }

            if (!File.Exists(Source))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed($"\"{Source}\" does not exist"));
                yield break;
            }

            if (File.Exists(Destination))
            {
                onActionPerformed?.Invoke(this, KnotModActionResult.Failed($"{Destination} already exist"));
                yield break;
            }

            File.Move(Source, Destination);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
        }
    }
}
