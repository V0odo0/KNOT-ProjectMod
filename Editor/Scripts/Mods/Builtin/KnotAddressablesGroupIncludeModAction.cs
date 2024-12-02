#if KNOT_ADDRESSABLES
using Knot.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    [KnotTypeInfo(displayName: "Addressables Groups Include", MenuCustomName = BuiltinModActionPath + "Addressables Groups Include", Order = 1800)]
    public class KnotAddressablesGroupIncludeModAction : KnotModActionBase
    {
        public List<AddressableAssetGroup> Groups => _groups;
        [SerializeField] private List<AddressableAssetGroup> _groups = new();

        public GroupAction Action
        {
            get => _action;
            set => _action = value;
        }
        [SerializeField] private GroupAction _action = GroupAction.IncludeInBuild;


        public override string GetDescription() => $"{Action.ToString()} {Groups.Count} group(s)";

        public override IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
        {
            foreach (var group in Groups)
            {
                if (group == null)
                    continue;

                var schema = group.GetSchema<BundledAssetGroupSchema>();
                if (schema == null) 
                    continue;

                schema.IncludeInBuild = Action == GroupAction.IncludeInBuild;
                EditorUtility.SetDirty(group);
            }

            onActionPerformed?.Invoke(this, KnotModActionResult.Completed());
            yield break;
        }

        [Serializable]
        public enum GroupAction
        {
            IncludeInBuild,
            ExcludeFromBuild
        }
    }
}
#endif